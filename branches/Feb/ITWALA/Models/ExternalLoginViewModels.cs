using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITWALA.Models
{
    public class Facebook
    {
        public string BaseUrl = "https://www.facebook.com/dialog/oauth?client_id=";
        public const string FacebookGraphApiToken = "https://graph.facebook.com/oauth/access_token?";
        public const string FacebookGraphApiMe = "https://graph.facebook.com/me?";

        public string AppId = "216740562037712";
        public string AppSecret = "c797d813dd5e2e0ac0d0ba4170876deb";
    }

    public class GoogleUserData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }
}