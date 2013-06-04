using System;
namespace YouConfWorker.Services.Email
{
    public interface IMailSender
    {
        void Send(string to, string subject, string htmlBody);
    }
}
