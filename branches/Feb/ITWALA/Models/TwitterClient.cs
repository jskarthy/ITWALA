using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITWALA.Models
{
    public class TwitterClient
    {
        public string BaseUrl { get; } = "https://api.twitter.com/oauth2/token";

        public string TokenUrl { get; } =
            "client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code";

        public string ApiMe { get; } = "https://www.googleapis.com/plus/v1/people/me?access_token={0}";
        // 581220499-fb9oe5s59tm56ttbr49slehb2gdol6fd.apps.googleusercontent.com - http://localhost/ITWALA/Account/TwitterLoginOK

        public string AppId { get; set; }

        public string AppSecret { get; set; }
        public string ProviderName => "Twitter";

        public TwitterClient()
        {
            AppId = "MEqs4vMiNDKhNYkbPp4ajZAvL";
            AppSecret = "UMMaVwP8aL4VUqO71o7ikEC5Og3h4r7jT0sDFuBgTaV3iHiLhT";
        }

        public TwitterClient(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
        }

        public AuthenticationResult VerifyAuthentication()
        {
            //var oauth_url = "https://api.twitter.com/oauth2/token";
            var oauth_url = "POST https://api.twitter.com/oauth/request_token?oauth_callback="+ HttpUtility.UrlEncode("http://localhost/ITWALA/Account/TwitterLoginOK");

            string consumerKey = "MEqs4vMiNDKhNYkbPp4ajZAvL";
            string consumerSecret = "UMMaVwP8aL4VUqO71o7ikEC5Og3h4r7jT0sDFuBgTaV3iHiLhT";
            var headerFormat = "Basic {0}";

            var authHeader = string.Format(headerFormat,
                 Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerKey) + ":" +
                        Uri.EscapeDataString((consumerSecret)))
                        ));

            var postBody = "grant_type=client_credentials";

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(oauth_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (Stream stream = request.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            request.Headers.Add("Accept-Encoding", "gzip");
            WebResponse response = request.GetResponse();
            Dictionary<string, string> userData;
            using (response)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objectText = reader.ReadToEnd();
                    userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(objectText);
                }
            }
            string accessToken = userData["access_token"];
            string url = "https://api.twitter.com/oauth/authorize?oauth_token={0}";
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(string.Format(url,accessToken));
            //var headerFormat2 = "{0} {1}";
            //request2.Headers.Add("Authorization", string.Format(headerFormat2, userData["token_type"], accessToken));
            request2.Method = "GET";
            WebResponse response2 = request2.GetResponse();

            // pull out the json string. we'll pass it back as a JSON.NET array instead of a nasty string
            string responseJson;
            using (response2)
            {
                using (var reader = new StreamReader(response2.GetResponseStream()))
                {
                    responseJson = reader.ReadToEnd();
                    userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);
                }
            }
            string id = userData["id"];
            string username = userData["email"];
            userData.Remove("id");
            userData.Remove("email");
            AuthenticationResult result = new AuthenticationResult(true, ProviderName, id, username, userData);
            return result;
        }

        public AuthenticationResult GetUserData()
        {
            string oauthcallback = "http://localhost/ITWALA/Account/TwitterLoginOK";
            string oauthconsumerkey = "MEqs4vMiNDKhNYkbPp4ajZAvL";
            string oauthconsumersecret = "UMMaVwP8aL4VUqO71o7ikEC5Og3h4r7jT0sDFuBgTaV3iHiLhT";
            string oauthtokensecret = string.Empty;
            string oauthtoken = string.Empty;
            string oauthsignaturemethod = "HMAC-SHA1";
            string oauthversion = "1.0";
            string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauthtimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            string url = "https://api.twitter.com/oauth/request_token?oauth_callback=" + oauthcallback;
            SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
            basestringParameters.Add("oauth_version", oauthversion);
            basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
            basestringParameters.Add("oauth_nonce", oauthnonce);
            basestringParameters.Add("oauth_signature_method", oauthsignaturemethod);
            basestringParameters.Add("oauth_timestamp", oauthtimestamp);
            basestringParameters.Add("oauth_callback", Uri.EscapeDataString(oauthcallback));

            //Build the signature string
            StringBuilder baseString = new StringBuilder();
            baseString.Append("POST" + "&");
            baseString.Append(EncodeCharacters(Uri.EscapeDataString(url.Split('?')[0]) + "&"));
            foreach (KeyValuePair<string, string> entry in basestringParameters)
            {
                baseString.Append(EncodeCharacters(Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&")));
            }

            //Remove the trailing ambersand char last 3 chars - %26
            string finalBaseString = baseString.ToString().Substring(0, baseString.Length - 3);

            //Build the signing key
            string signingKey = EncodeCharacters(Uri.EscapeDataString(oauthconsumersecret)) + "&" +
            EncodeCharacters(Uri.EscapeDataString(oauthtokensecret));

            //Sign the request
            HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
            string oauthsignature = Convert.ToBase64String(
              hasher.ComputeHash(new ASCIIEncoding().GetBytes(finalBaseString)));

            //Tell Twitter we don't do the 100 continue thing
            ServicePointManager.Expect100Continue = false;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@url);

            StringBuilder authorizationHeaderParams = new StringBuilder();
            authorizationHeaderParams.Append("OAuth ");
            authorizationHeaderParams.Append("oauth_nonce=" + "\"" + Uri.EscapeDataString(oauthnonce) + "\",");
            authorizationHeaderParams.Append("oauth_signature_method=" + "\"" + Uri.EscapeDataString(oauthsignaturemethod) + "\",");
            authorizationHeaderParams.Append("oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauthtimestamp) + "\",");
            authorizationHeaderParams.Append("oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauthconsumerkey) + "\",");
            authorizationHeaderParams.Append("oauth_signature=" + "\"" + Uri.EscapeDataString(oauthsignature) + "\",");
            authorizationHeaderParams.Append("oauth_version=" + "\"" + Uri.EscapeDataString(oauthversion) + "\"");
            webRequest.Headers.Add("Authorization", authorizationHeaderParams.ToString());

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            //Allow us a reasonable timeout in case Twitter's busy
            webRequest.Timeout = 3 * 60 * 1000;

            try
            {
                //webRequest.Proxy = new WebProxy("enter proxy details/address");
                HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;
                Stream dataStream = webResponse.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                IDictionary<string,string> userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseFromServer);
                string id = userData["id"];
                string username = userData["email"];
                userData.Remove("id");
                userData.Remove("email");
                AuthenticationResult result = new AuthenticationResult(true, ProviderName, id, username, userData);
                return result;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        private string EncodeCharacters(string data)
        {
            //as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
            //unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
            if (data.Contains("!"))
                data = data.Replace("!", "%21");
            if (data.Contains("'"))
                data = data.Replace("'", "%27");
            if (data.Contains("("))
                data = data.Replace("(", "%28");
            if (data.Contains(")"))
                data = data.Replace(")", "%29");
            if (data.Contains("*"))
                data = data.Replace("*", "%2A");
            if (data.Contains(","))
                data = data.Replace(",", "%2C");

            return data;
        }
        private string GetHtml(string url)
        {
            string connectionString = url;
            string sURL = "https://www.googleapis.com/oauth2/v4/token";
            try
            {
                WebRequest myRequest = WebRequest.Create(sURL);
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                using (Stream stream = myRequest.GetRequestStream())
                {
                    byte[] content = ASCIIEncoding.ASCII.GetBytes(connectionString);
                    stream.Write(content, 0, content.Length);
                }
                //myRequest.Credentials = CredentialCache.DefaultCredentials;
                //// Get the response
                WebResponse webResponse = myRequest.GetResponse();
                Stream responseStream = webResponse.GetResponseStream();
                ////
                if (responseStream != null)
                {
                    StreamReader ioStream = new StreamReader(responseStream);
                    string pageContent = ioStream.ReadToEnd();
                    //// Close streams
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