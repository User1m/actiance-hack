﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//// The following using statements were added for this sample.
//using Owin;
//using Microsoft.Owin.Security;
//using System.Configuration;
//using System.Globalization;
//using System.Threading.Tasks;
//using Microsoft.Owin.Security.ActiveDirectory;
//using Microsoft.Owin.Security.Cookies;

//namespace Actiance
//{
//    public partial class Startup
//    {
//        //
//        // The Client ID is used by the application to uniquely identify itself to Azure AD.
//        // The Metadata Address is used by the application to retrieve the signing keys used by Azure AD.
//        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
//        // The Authority is the sign-in URL of the tenant.
//        // The Post Logout Redirect Uri is the URL where the user will be redirected after they sign out.
//        //
//        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
//        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
//        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
//        //private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
//        //private static string authority = ConfigurationManager.AppSettings["ida:Authority"];
//        private static string metadata = string.Format("{0}/{1}/federationmetadata/2007-06/federationmetadata.xml", aadInstance, tenant);

//        string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

//        public void ConfigureAuth(IAppBuilder app)
//        {
//            Console.WriteLine(authority);

//            // TODO: Hook up OWIN to perform OpenID Connect Authentication

//            //app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

//            //app.UseCookieAuthentication(new CookieAuthenticationOptions());

//            app.UseActiveDirectoryFederationServicesBearerAuthentication(
//                new ActiveDirectoryFederationServicesBearerAuthenticationOptions
//                {
//                    Audience = ConfigurationManager.AppSettings["ida:Audience"],
//                    MetadataEndpoint = metadata
//                }
//            );

//            //app.UseOpenIdConnectAuthentication(
//            //new OpenIdConnectAuthenticationOptions
//            //{
//            //    ClientId = clientId,
//            //    Authority = authority,
//            //    PostLogoutRedirectUri = postLogoutRedirectUri,
//            //    Notifications = new OpenIdConnectAuthenticationNotifications
//            //    {
//            //        AuthenticationFailed = context =>
//            //        {
//            //            context.HandleResponse();
//            //            context.Response.Redirect("/Error?message=" + context.Exception.Message);
//            //            return Task.FromResult(0);
//            //        }
//            //    }
//            //});
//        }
//    }
//}


