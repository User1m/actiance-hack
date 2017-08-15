﻿using System.Web.Http;
using Actiance.Controllers;

namespace Actiance
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            if (AuthController.NeedsOauthToken())
            {
                AuthController.AdminConsent();
            }
        }
    }
}
