using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

// The following using statements were added for this sample.

using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Actiance.Models;
using Actiance.Services;

namespace Actiance.Controllers
{
    public class AuthController : ApiController
    {

        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string msGraph = ConfigurationManager.AppSettings["ida:MSGraph"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        public static string authTenant = ConfigurationManager.AppSettings["ida:AuthTenant"];
        private static string scope = ConfigurationManager.AppSettings["ida:Scope"];
        //private static string metadata = string.Format("{0}/{1}/federationmetadata/2007-06/federationmetadata.xml", aadInstance, tenant);
        static string authority = $"{aadInstance}/{tenant}/adminconsent?client_id={clientId}&state=auth&redirect_uri={redirectUri}";
        //eg. "https://login.microsoftonline.com/actiancehack.onmicrosoft.com/adminconsent?client_id=e5b5c8c1-2b25-4437-ba24-98d665a10f05&state=12345&redirect_uri=https://5b59e015.ngrok.io"



        //AUTH CREDENTIALS
        private static string oauthToken = "";
        static IEnumerable<KeyValuePair<string, string>> postData = new Dictionary<string, string> {
            { "client_id", clientId },
            { "scope", $"{msGraph}/{scope}" },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" }
        };


        public static void GetAdminConsent()
        {
            System.Diagnostics.Process.Start(authority);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string result = "Success! Close this page!";
            string failure = "Something went wrong. Please restart the Bot";

            var queryString = this.Request.GetQueryNameValuePairs();
            authTenant = queryString.FirstOrDefault(x => x.Key == "tenant").Value;

            await GetToken();

            //APIService.GetUsers();

            return (string.IsNullOrEmpty(authTenant) ? failure : result);
        }

        public async Task GetToken()
        {
            await PostForOauthToken();
        }

        public static async Task<string> GetOauthToken()
        {
            if (string.IsNullOrEmpty(oauthToken))
            {
                await PostForOauthToken();
                //GetAdminConsent();
                //return string.Empty;
            }
            return oauthToken;
        }

        /// <summary>
        /// Post to https://login.microsoftonline.com/db35d98a-b61b-4362-90e6-22237a243507/oauth2/v2.0/token
        /// </summary>
        /// <returns></returns>
        public static async Task PostForOauthToken()
        {
            TokenResponse tokenResponse = await APIService.PostTo<TokenResponse>($"{aadInstance}/{authTenant}/oauth2/v2.0/token", postData);
            oauthToken = tokenResponse.access_token;
        }

        public static bool NeedsOauthToken()
        {
            return string.IsNullOrEmpty(authTenant);
        }

    }
}