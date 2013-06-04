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
        int _minPollInterval = 5000;
        int _maxPollInterval = 300000;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        bool IsStopped;

        public override void Run()
        {
            while (!IsStopped)
            {
                try
                {
                    // Receive as many messages as possible (to reduce the number of storage transactions)
                    var receivedMessages = Client.ReceiveBatch(32);
                    if (receivedMessages.Count() == 0)
                    {
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
                            Trace.TraceError("Message does not have a messagebodytype specified, message {0}", receivedMessage.MessageId);
                            receivedMessage.DeadLetter();
                        }

                        MethodInfo method = typeof(BrokeredMessage).GetMethod("GetBody", new Type[] { });
                        MethodInfo generic = method.MakeGenericMethod(messageBodyType);
                        var messageBody = generic.Invoke(receivedMessage, null);

                        CallMessageHandler(messageBody);

                        receivedMessage.Complete();
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }

                    Thread.Sleep(10000);
                }
                catch (OperationCanceledException e)
                {
                    if (!IsStopped)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                }
            }
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
            Client.Close();
            base.OnStop();
        }

        public void CallMessageHandler<T>(T message) where T : class
        {
            Type handlerType = typeof(IMessageHandler<>);
            Type[] typeArgs = { message.GetType() };
            Type constructed = handlerType.MakeGenericType(typeArgs);

            var handler = NinjectKernel.Get(constructed);

            var methodInfo = constructed.GetMethod("Handle");
            methodInfo.Invoke(handler, new[] { message });
        }
    }
}
