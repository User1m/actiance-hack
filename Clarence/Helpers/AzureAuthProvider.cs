using System;
using System.Net.Http;
using System.Threading.Tasks;
using Actiance.Controllers;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Actiance.Helpers
{
    class AzureAuthenticationProvider : IAuthenticationProvider
    {
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            //string clientId = "ae798feb-2a57-4738-8037-2e7d57ac6930";
            //string clientSecret = "84fxdktLxiXvax0iI/m1ARB+QaEpd2c8jZD6tQK9Alc=";
            //AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/jonhussdev.onmicrosoft.com/oauth2/token");

            AuthenticationContext authContext = new AuthenticationContext($"{AuthController.aadInstance}/{AuthController.authTenant}/oauth2/v2.0/token");
            ClientCredential creds = new ClientCredential(AuthController.clientId, AuthController.clientSecret);
            AuthenticationResult authResult = await authContext.AcquireTokenAsync($"{AuthController.msGraph}/", creds);
            request.Headers.Add("Authorization", $"Bearer {authResult.AccessToken}");
        }
    }
}