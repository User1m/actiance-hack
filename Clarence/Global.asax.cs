using System.Web.Http;
using Actiance.Controllers;
using Actiance.Services;
using Microsoft.Bot.Builder.Dialogs;
using Actiance.Helpers;
using Autofac;

namespace Actiance
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Conversation.UpdateContainer(builder =>
            //{
            //    builder.RegisterType<Middleware>().AsImplementedInterfaces().InstancePerDependency();
            //});
            var builder = new ContainerBuilder();
            builder.RegisterType<Middleware>().AsImplementedInterfaces().InstancePerDependency();
            builder.Update(Conversation.Container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            if (AuthController.NeedsOauthToken())
            {
                AuthController.GetAdminConsent();
            }
            //else
            //{
            //    APIService.GetUsers();
            //}
        }
    }


}
