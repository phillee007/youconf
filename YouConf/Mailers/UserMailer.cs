using Mvc.Mailer;

namespace YouConf.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}

        public virtual MvcMailMessage Welcome(string username)
		{
            ViewBag.Username = username;
			return Populate(x =>
			{
				x.Subject = "Welcome to YouConf";
				x.ViewName = "Welcome";
				x.To.Add("some-email@example.com");
			});
		}
 
		public virtual MvcMailMessage PasswordReset(string email, string username, string token)
        {
            ViewBag.Token = token;
            ViewBag.Username = username;
            return Populate(x =>
            {
                x.Subject = "Reset your password";
                x.ViewName = "PasswordReset";
            });
        }
 	}
}