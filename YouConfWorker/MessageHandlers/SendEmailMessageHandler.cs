using AzureDemo.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using YouConf.Common.Messaging;
using YouConfWorker.Services.Email;

namespace AzureDemo.WindowsService.MessageHandlers
{
    public class SendEmailMessageHandler : IMessageHandler<SendEmailMessage>
    {
        public IMailSender Mailer { get; set; }
        public SendEmailMessageHandler(IMailSender mailer)
        {
            if (mailer == null)
            {
                throw new ArgumentNullException("mailer");
            }
            Mailer = mailer;
        }

        public void Handle(SendEmailMessage message)
        {
            Mailer.Send(message.To, message.Subject, message.Body);
        }
    }
}
