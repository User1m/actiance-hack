//using System;
//using Owin;
//using Microsoft.Owin;
//using System.Threading.Tasks;
//using Owin;
//using Microsoft.Owin.Security;
//using System.Configuration;
//using System.Globalization;
//using System.Threading.Tasks;
//using Microsoft.Owin.Security.ActiveDirectory;
//using Microsoft.Owin.Security.Cookies;
//using Actiance.Controllers;

////[assembly: OwinStartup(typeof(Actiance.Startup))]

//namespace Actiance
//{
//    public partial class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            //app.UseWelcomePage();
//            //ConfigureAuth(app);
//        }

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
