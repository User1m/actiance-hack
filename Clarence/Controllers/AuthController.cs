﻿using System;
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

        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        private static string authTenant = ConfigurationManager.AppSettings["ida:AuthTenant"];
        private static string scope = ConfigurationManager.AppSettings["ida:Scope"];
        //private static string metadata = string.Format("{0}/{1}/federationmetadata/2007-06/federationmetadata.xml", aadInstance, tenant);

        static string authority = string.Format("{0}/{1}/adminconsent?client_id={2}&state=auth&redirect_uri={3}", aadInstance, tenant, clientId, redirectUri);
        //eg. "https://login.microsoftonline.com/actiancehack.onmicrosoft.com/adminconsent?client_id=e5b5c8c1-2b25-4437-ba24-98d665a10f05&state=12345&redirect_uri=https://5b59e015.ngrok.io"

        static string tokenEndpoint = "";

        //AUTH CREDENTIALS
        private static string oauthToken = "";
        private static string tenantId = "";
        IEnumerable<KeyValuePair<string, string>> postData = new Dictionary<string, string> {
            { "client_id", clientId },
            { "scope", scope },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" }
        };


        public static void AdminConsent()
        {
            System.Diagnostics.Process.Start(authority);
        }


        [HttpGet]
        public async Task<string> Get()
        {
            string result = "Success! Close this page!";
            string failure = "Something went wrong. Please restart the Bot";

            var queryString = this.Request.GetQueryNameValuePairs();
            tenantId = queryString.FirstOrDefault(x => x.Key == "tenant").Value;

            tokenEndpoint = string.Format("{0}/{1}/oauth2/v2.0/token", aadInstance, tenantId);

            await PostForOauthToken();

            return (string.IsNullOrEmpty(AuthController.tenantId) ? failure : result);

        }


        public async Task<string> GetOauthToken()
        {
            if (string.IsNullOrEmpty(oauthToken))
            {
                await PostForOauthToken();
            }
            return oauthToken;
        }


        public async Task PostForOauthToken()
        {
            //post to https://login.microsoftonline.com/db35d98a-b61b-4362-90e6-22237a243507/oauth2/v2.0/token
            TokenResponse tokenResponse = await APIService.PostTo<TokenResponse>(tokenEndpoint, postData);
            oauthToken = tokenResponse.access_token;
            //Console.WriteLine(oauthToken);
        }

        public static bool NeedsOauthToken()
        {
            return string.IsNullOrEmpty(authTenant);
        }

    }
}