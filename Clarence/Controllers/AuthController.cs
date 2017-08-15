using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

// The following using statements were added for this sample.

using System.Web.Http;

namespace Actiance.Controllers
{
    public class AuthController : ApiController
    {

        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        //private static string metadata = string.Format("{0}/{1}/federationmetadata/2007-06/federationmetadata.xml", aadInstance, tenant);

        static string authority = string.Format("https://{0}/{1}/adminconsent?client_id={3}&state=auth&redirect_uri={4}", aadInstance, tenant, clientId, redirectUri);
        //eg. "https://login.microsoftonline.com/actiancehack.onmicrosoft.com/adminconsent?client_id=e5b5c8c1-2b25-4437-ba24-98d665a10f05&state=12345&redirect_uri=https://5b59e015.ngrok.io"

        public static void AdminConsent()
        {
            System.Diagnostics.Process.Start(authority);
        }

        public AuthController()
        {

        }

        public IHttpActionResult Get(string test)
        {
            return Ok("it worked");
        }


        public string PostForOauthToken()
        {
            return "bearer";
        }

    }
}