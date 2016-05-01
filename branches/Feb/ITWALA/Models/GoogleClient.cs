using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITWALA.Models
{
    public class GoogleClient
    {
        public readonly string RedirectUrl = "http://localhost/ITWALA/Account/GoogleLoginOK";

        public readonly string BaseUrl =
            "https://accounts.google.com/o/oauth2/auth?response_type=code&scope=https://www.googleapis.com/auth/userinfo.email&client_id={0}&redirect_uri={1}";

        private readonly string _tokenData =
            "client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code";
        private string _tokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        private readonly string _userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token={0}";

        public string AppId { get; set; }

        public string AppSecret { get; set; }
        public string ProviderName => "Google";

        public GoogleClient()
        {
            AppId = "581220499-fb9oe5s59tm56ttbr49slehb2gdol6fd.apps.googleusercontent.com";
            AppSecret = "69p8rap2sRVZNx5LNmahv14N";
        }

        public GoogleClient(string appId,string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public GoogleUserViewModel VerifyAuthentication(string code)
        {
            
            GoogleUserViewModel userData = GetUserData(code, RedirectUrl);

            if (userData == null) return null;
                //return new AuthenticationResult(false, ProviderName, null, null, null);

            //AuthenticationResult result = new AuthenticationResult(true, ProviderName, id, username, user);
            return userData;
        }

        private GoogleUserViewModel GetUserData(string code, string redirectUrl)
        {
            string token = GetAccessToken(string.Format(_tokenData, AppId, redirectUrl, AppSecret, code));
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            JObject obj = JObject.Parse(token);
            string accessToken = obj["access_token"].ToString();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(string.Format(_userInfoUrl,accessToken));
            webRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            if (responseStream != null)
            {
                StreamReader reader = new StreamReader(responseStream);
                string data = reader.ReadToEnd();
                GoogleUserViewModel userData = JsonConvert.DeserializeObject<GoogleUserViewModel>(data);
                return userData;
            }
            else
            {
                return null;
            }
        }

        private string GetAccessToken(string url)
        {
            string connectionString = url;
            try
            {
                WebRequest myRequest = WebRequest.Create(_tokenUrl);
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                using(Stream stream = myRequest.GetRequestStream())
                {
                    byte[] content = ASCIIEncoding.ASCII.GetBytes(connectionString);
                    stream.Write(content, 0, content.Length);
                }
                WebResponse webResponse = myRequest.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();
                ////
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
    }
}