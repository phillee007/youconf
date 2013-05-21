using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using YouConf.Models;
using System.Configuration;

namespace YouConf
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166
            Dictionary<string, object> microsoftSocialData = new Dictionary<string, object>();
            microsoftSocialData.Add("Icon", "/images/icons/social/microsoft.png");
            OAuthWebSecurity.RegisterMicrosoftClient(
                clientId: ConfigurationManager.AppSettings["Auth-MicrosoftAuthClientId"],
                clientSecret: ConfigurationManager.AppSettings["Auth-MicrosoftAuthClientSecret"],
                displayName: "Windows Live",
                extraData: microsoftSocialData);

            Dictionary<string, object> googleSocialData = new Dictionary<string, object>();
            googleSocialData.Add("Icon", "/images/icons/social/google.png");
            OAuthWebSecurity.RegisterGoogleClient("Google", googleSocialData);

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");
        }
    }
}
