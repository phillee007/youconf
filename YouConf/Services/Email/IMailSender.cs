using System;
namespace YouConf.Services.Email
{
    public interface IMailSender
    {
        void Send(string to, string subject, string htmlBody);
    }
}
