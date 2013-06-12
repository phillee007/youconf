using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Ninject;
using Ninject.Extensions.Conventions;
using YouConf.Common.Messaging;
using System.Reflection;
using YouConf.Common.Data;
using YouConfWorker.Services.Email;
using Ninject.Integration.SolrNet;

namespace YouConfWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IKernel NinjectKernel;
        // The name of your queue
        const string QueueName = "ProcessingQueue";
        int _currentPollInterval = 5000;
        //One second
        int _minPollInterval = 1000;
        //120 seconds
        int _maxPollInterval = 120000;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        bool IsStopped;
        private volatile bool _returnedFromRunMethod = false;

        public override void Run()
        {
            while (!IsStopped)
            {
                BrokeredMessage msg = null;
                try
                {
                    // Receive as many messages as possible (to reduce the number of storage transactions)
                    var receivedMessages = Client.ReceiveBatch(32);
                    if (receivedMessages.Count() == 0)
                    {
                        Thread.Sleep(_currentPollInterval);
                        //No messages, so increase poll interval
                        if (_currentPollInterval < _maxPollInterval)
                        {
                            _currentPollInterval = _currentPollInterval * 2;
                        }
                        continue;
                    }

                    //At least one message, so reset our poll interval
                    _currentPollInterval = _minPollInterval;

                    foreach (var receivedMessage in receivedMessages)
                    {
                        msg = receivedMessage;

                        // Process the message
                        Trace.WriteLine("Processing", receivedMessage.SequenceNumber.ToString());

                        //If it's a poison message, move it off to the deadletter queue
                        if (receivedMessage.DeliveryCount > 3)
                        {
                            Trace.TraceError("Deadlettering poison message: message {0}", receivedMessage.ToString());
                            receivedMessage.DeadLetter();
                            continue;
                        }

                        //Get actual message type
                        var messageBodyType = Type.GetType(receivedMessage.Properties["messageType"].ToString());
                        if (messageBodyType == null)
                        {
                            //Should never get here as a messagebodytype should always be set BEFORE putting the message on the queue
                            Trace.TraceError("Message does not have a messagebodytype specified, message {0}", receivedMessage.ToString());
                            receivedMessage.DeadLetter();
                        }

                        //Use reflection to figure out the type of object contained in the message body, and extract it
                        MethodInfo method = typeof(BrokeredMessage).GetMethod("GetBody", new Type[] { });
                        MethodInfo generic = method.MakeGenericMethod(messageBodyType);
                        var messageBody = generic.Invoke(receivedMessage, null);

                        //Process the message contents
                        ProcessMessage(messageBody);

                        //Everything ok, so take it off the queue
                        receivedMessage.Complete();
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.ToString());
                    }

                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                    if (ex.InnerException != null)
                    {
                        err += "\r\n Inner Exception: " + ex.InnerException.ToString();
                    }
                    if (msg != null)
                    {
                        err += "\r\n Last queue message retrieved: " + msg.ToString();
                    }
                    Trace.TraceError(err);
                    // Don't fill up Trace storage if we have a bug in either process loop.
                    System.Threading.Thread.Sleep(1000 * 60);
                }
            }

            // If OnStop has been called, return to do a graceful shutdown.
            _returnedFromRunMethod = true;
            Trace.WriteLine("Exiting run method");
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //Diagnostics
            ConfigureDiagnostics();

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            IsStopped = false;

            ConfigureNinject();

            return base.OnStart();
        }

        private void ConfigureDiagnostics()
        {
            DiagnosticMonitorConfiguration config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            config.ConfigurationChangePollInterval = TimeSpan.FromMinutes(1d);
            config.Logs.BufferQuotaInMB = 500;
            config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1d);

            DiagnosticMonitor.Start(
                   "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString",
                   config);
        }

        private void ConfigureNinject()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IYouConfDbContext>()
                .To<YouConfDbContext>();
            kernel.Bind<IMailSender>()
                .To<SmtpMailSender>();
            kernel.Load(new SolrNetModule("http://youconfsearch.cloudapp.net/solr"));

            kernel.Bind(x => x.FromThisAssembly()
                  .SelectAllClasses().InheritedFrom(typeof(IMessageHandler<>))
                  .BindAllInterfaces());

            NinjectKernel = kernel;
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
            while (_returnedFromRunMethod == false)
            {
                System.Threading.Thread.Sleep(1000);
            }
            Client.Close();
            base.OnStop();
        }

        /// <summary>
        /// Locates the correct handler type, and executes it using the current message
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="message">The actual message body</param>
        public void ProcessMessage<T>(T message) where T : class
        {
            //Voodoo to construct the right message handler type
            Type handlerType = typeof(IMessageHandler<>);
            Type[] typeArgs = { message.GetType() };
            Type constructed = handlerType.MakeGenericType(typeArgs);
            //Get an instance of the message handler type
            var handler = NinjectKernel.Get(constructed);

            //Handle the message
            var methodInfo = constructed.GetMethod("Handle");
            methodInfo.Invoke(handler, new[] { message });
        }
    }
}
