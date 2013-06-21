using Mvc.Mailer;

namespace YouConf.Mailers
{ 
    public interface IUserMailer
    {
			MvcMailMessage Welcome(string username);
            MvcMailMessage PasswordReset(string email, string username, string token);
	}
}