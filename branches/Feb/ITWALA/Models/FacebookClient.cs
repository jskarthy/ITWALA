using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;
using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;

namespace ITWALA.Models
{
    public class FacebookClient
    {
        private string _appId;
        private string _appSecret;
            
        public readonly string RedirectUrl = "http://localhost/ITWALA/Account/FacebookLoginCallback";

        //private const string BaseUrl = "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}";

        public string BaseUrl { get; } = "https://graph.facebook.com/oauth/authorize?type=web_server&client_id={0}&redirect_uri={1}";

        public string GraphApiToken { get; } = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";
        public string GraphApiMe { get; } = "https://graph.facebook.com/me?fields=id,first_name,last_name,name,email,link&";

        public string AppId {
            get { return _appId; }
            set { _appId = value; }
        }
        public string AppSecret
        {
            get { return _appSecret; }
            set { _appSecret = value; }
        }
        public FacebookClient()
        {
            //AppId = "216740562037712";
            //AppSecret = "c797d813dd5e2e0ac0d0ba4170876deb";
            AppId = "980126235387167";
            AppSecret = "1742635016fdd309e85214cb8705ab95";
        }

        public FacebookClient(string appId,string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public string ProviderName
        {
            get { return "Facebook"; }
        }
        private static string GetHtml(string url)
        {
            try
            {
                WebRequest myRequest = WebRequest.Create(url);
                WebResponse webResponse = myRequest.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();
                if (responseStream != null)
                {
                    StreamReader ioStream = new StreamReader(responseStream);
                    string pageContent = ioStream.ReadToEnd();
                    ioStream.Close();
                    responseStream.Close();
                    return pageContent;
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            return null;
        }

        private FacebookUserViewModel GetUserData(string accessCode, string redirectUri)
        {
            string token = GetHtml(string.Format(GraphApiToken,AppId,redirectUri,AppSecret,accessCode));
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            string data = GetHtml(GraphApiMe + token);

            FacebookUserViewModel userData = JsonConvert.DeserializeObject<FacebookUserViewModel>(data);
            return userData;
        }

        public FacebookUserViewModel VerifyAuthentication(string code)
        {
            FacebookUserViewModel userData = GetUserData(code, RedirectUrl);
            return userData;
        }
    }
}