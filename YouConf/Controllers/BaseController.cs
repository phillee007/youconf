using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Common.Messaging;

namespace YouConf.Controllers
{
    public class BaseController : Controller
    {
        const string QueueName = "ProcessingQueue";

        protected void SendQueueMessage<T>(T message)
        {
            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            var client = QueueClient.CreateFromConnectionString(connectionString, QueueName);

            // Create message, passing a string message for the body
            BrokeredMessage brokeredMessage = new BrokeredMessage(message);
            brokeredMessage.Properties["messageType"] = message.GetType().AssemblyQualifiedName;
            client.Send(brokeredMessage);
        }

        protected void UpdateConferenceInSolrIndex(int conferenceId, SolrIndexAction action)
        {
            var message = new UpdateSolrIndexMessage()
            {
                ConferenceId = conferenceId,
                Action = action
            };
            SendQueueMessage(message);
        }

    }
}
