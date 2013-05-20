using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace YouConf.Services.Email
{
    public class SmtpMailSender : YouConf.Services.Email.IMailSender
    {
        public void Send(string to, string subject, string htmlBody)
        {
            MailMessage mailMsg = new MailMessage();

            // To
            mailMsg.To.Add(new MailAddress(to));

            // From
            mailMsg.From = new MailAddress("no-reply@youconf.azurewebsites.net", "YouConf support");

            // Subject and multipart/alternative Body
            mailMsg.Subject = subject;
            string text = "You need an html-capable email viewer to read this";
            string html = htmlBody;
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            // Init SmtpClient and send
            SmtpClient smtpClient = new SmtpClient();
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(CloudConfigurationManager.GetSetting("Sendgrid.Username"), CloudConfigurationManager.GetSetting("Sendgrid.Password"));
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);
        }
    }
}