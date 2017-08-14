/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraphWebhooks.Helpers
{
    public class AuthService
    {
        private static string aadInstance = ConfigurationManager.AppSettings["AadInstance"];
        private static string appId = ConfigurationManager.AppSettings["ClientId"];
        private static string appSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private static string graphResourceId = ConfigurationManager.AppSettings["GraphResourceId"];
        
        private static string TokenForApplication { get; set; }
        
        /// <summary>
        /// Get Token for Application.
        /// </summary>
        /// <returns>Token for application.</returns>
        public static async Task<string> GetTokenForApplication()
        {
            if (TokenForApplication == null)
            {
                AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance, false);
                ClientCredential clientCred = new ClientCredential(appId, appSecret);
                AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(graphResourceId, clientCred);
                TokenForApplication = authenticationResult.AccessToken;
            }
            return TokenForApplication;
        }
        

    }
}