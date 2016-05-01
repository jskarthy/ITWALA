using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITWALA.Models;
using Microsoft.Web.WebPages.OAuth;

namespace ITWALA.App_Start
{
    public class OAuthConfig
    {
        public static void RegisterProviders()
        {

            var fb = new Dictionary<string, object>();
            fb.Add("scope", "email");
            OAuthWebSecurity.RegisterGoogleClient();
            //OAuthWebSecurity.RegisterFacebookClient("980126235387167", "1742635016fdd309e85214cb8705ab95");
            //OAuthWebSecurity.RegisterFacebookClient("216740562037712",
            //"c797d813dd5e2e0ac0d0ba4170876deb","Facebook",fb);
            //OAuthWebSecurity.RegisterClient(new FacebookClient("216740562037712",
                //"c797d813dd5e2e0ac0d0ba4170876deb"));
            OAuthWebSecurity.RegisterTwitterClient("MEqs4vMiNDKhNYkbPp4ajZAvL", "UMMaVwP8aL4VUqO71o7ikEC5Og3h4r7jT0sDFuBgTaV3iHiLhT");
        }
    }
}