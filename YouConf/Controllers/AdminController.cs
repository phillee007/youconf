using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using YouConf.Common.Data;
using YouConf.Common.Messaging;

namespace YouConf.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : BaseController
    {
        public IYouConfDbContext YouConfDbContext { get; set; }

        public AdminController(IYouConfDbContext youConfDbContext)
        {
            if (youConfDbContext == null)
            {
                throw new ArgumentNullException("youConfDbContext");
            }
            YouConfDbContext = youConfDbContext;
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReIndex()
        {
            var conferences = YouConfDbContext.Conferences.ToList();
            foreach (var conference in conferences)
            {
                UpdateConferenceInSolrIndex(conference.Id, SolrIndexAction.Update);
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult DeleteAll()
        {
            var conferences = YouConfDbContext.Conferences.ToList();
            foreach (var conference in conferences)
            {
                UpdateConferenceInSolrIndex(conference.Id, SolrIndexAction.Delete);
            }

            return View("Index");
        }

        public ActionResult DeadletterQueue()
        {
            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var client = QueueClient.CreateFromConnectionString(connectionString, "ProcessingQueue");
            QueueClient deadLetterClient = QueueClient.Create(QueueClient.FormatDeadLetterPath(client.Path), ReceiveMode.PeekLock);

            IEnumerable<BrokeredMessage> receivedDeadLetterMessages;
            var poisonMessages = new List<string>();
            while ((receivedDeadLetterMessages = deadLetterClient.ReceiveBatch(32, TimeSpan.FromSeconds(10))) != null)
            {
                foreach (var receivedDeadLetterMessage in receivedDeadLetterMessages)
                {
                    var messageBodyType = Type.GetType(receivedDeadLetterMessage.Properties["messageType"].ToString());
                    if (messageBodyType == null)
                    {
                        poisonMessages.Add(receivedDeadLetterMessage.MessageId + ": no message body type specified");
                        continue;
                    }

                    //Use reflection to figure out the type of object contained in the message body, and extract it
                    MethodInfo method = typeof(BrokeredMessage).GetMethod("GetBody", new Type[] { });
                    MethodInfo generic = method.MakeGenericMethod(messageBodyType);
                    var messageBody = generic.Invoke(receivedDeadLetterMessage, null);
                    poisonMessages.Add(JsonConvert.SerializeObject(messageBody));
                }
                if (receivedDeadLetterMessages.Count() == 0)
                {
                    break;
                }
            }

            return View(poisonMessages);
        }
    }
}
