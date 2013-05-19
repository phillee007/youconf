using Mvc.Mailer;

namespace YouConf.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}
		
		public virtual MvcMailMessage Welcome()
		{
			//ViewBag.Data = someObject;
			return Populate(x =>
			{
				x.Subject = "Welcome";
				x.ViewName = "Welcome";
				x.To.Add("some-email@example.com");
			});
		}
 
		public virtual MvcMailMessage PasswordReset(string email, string token)
        {
            ViewBag.Token = token;
            return Populate(x =>
            {
                x.Subject = "Reset your password";
                x.ViewName = "PasswordReset";
            });
        }
 	}
}