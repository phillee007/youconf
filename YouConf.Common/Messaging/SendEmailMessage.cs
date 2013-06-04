using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDemo.Messaging
{
    public class SendEmailMessage
    {
        public string Sender{ get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
    }
}
