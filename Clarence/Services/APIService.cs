using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Actiance.Controllers;
using System.Globalization;
using System.Configuration;
using Actiance.Helpers;
using Newtonsoft.Json;
using Actiance.Models;

namespace Actiance.Services
{
    public class APIService
    {
        public APIService() { }

        private static string graphv1Endpoint = $"{AuthController.msGraph}/v1.0";
        private static string graphBetaEndpoint = $"{AuthController.msGraph}/beta";

        public static async Task<TResult> GetFrom<TResult>(string url, string token)
        {
            //get from https://graph.microsoft.com/
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                return await response.Content.ReadAsAsync<TResult>();
            }
        }

        public static async Task<TResult> PostTo<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            //post to https://login.microsoftonline.com/db35d98a-b61b-4362-90e6-22237a243507/oauth2/v2.0/token

            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    return await response.Content.ReadAsAsync<TResult>();
                }
            }
        }

        public static async Task GetUsers()
        {
            Storage.userStore = await GetFrom<List<User>>($"{graphv1Endpoint}/users/", await AuthController.GetOauthToken());
        }

        public static async Task<User> GetUser(string userName)
        {
            User user = await GetFrom<User>($"{graphv1Endpoint}/users/{userName}@{AuthController.tenant}", await AuthController.GetOauthToken());
            return user;
        }

        public static async Task<User> GetManager(string userId)
        {
            User manager = await GetFrom<User>($"{graphv1Endpoint}/users/{userId}/Manager/", await AuthController.GetOauthToken());
            return manager;
        }

        public static async Task<Dictionary<string, List<Message>>> GetMessageDeltasForUser(string userId)
        {

            var graphClient = new GraphServiceClient(
                graphBetaEndpoint,
                new AzureAuthenticationProvider()
                //new DelegateAuthenticationProvider(
                //async (requestMessage) =>
                //{
                //    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await AuthController.GetOauthToken());

                //    return Task.FromResult(0);
                //})
                );

            //get https://graph.microsoft.com/beta/users/clmb@actiancehack.onmicrosoft.com/mailFolders('teamchat')/messages/delta?$filter=receivedDateTime ge 2017-08-17T03:43:08Z
            //List<MessageDeltaCollectionResponse> messages = await GetFrom<List<MessageDeltaCollectionResponse>>($"{graphBetaEndpoint}/users/{userId}/mailFolders('teamchat')/messages/delta?$filter=receivedDateTime ge {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")}", await AuthController.GetOauthToken());
            //         var odata;
            //         using (var client = new HttpClient())
            //{
            //	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await AuthController.GetOauthToken());

            //	var json = await client.GetStringAsync(($"{graphBetaEndpoint}/users/{userId}/mailFolders('teamchat')/messages/delta?$filter=receivedDateTime ge {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")}"));
            //              JsonConvert.DeserializeObject<MessageDeltaQueryResponse>(json);
            //}

            List<Message> messages = new List<Message>();
            // Get our first delta page.
            string filter = $"receivedDateTime ge {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")}";
            var messagesDeltaCollectionPage = await graphClient.Users[Storage.user.Id].MailFolders["teamchat"].Messages.Delta().Request().Filter(filter).GetAsync();

            // Go through all of the delta pages so that we can get the delta link on the last page.
            while (messagesDeltaCollectionPage.NextPageRequest != null)
            {
                messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();
                foreach (var message in messagesDeltaCollectionPage)
                {
                    messages.Add(message);
                }
            }

            // At this point we're up to date. messagesDeltaCollectionPage now has a deltalink.  
            object deltaLink;

            // Now let's use the deltalink to make sure there aren't any changes. There shouldn't be.
            if (messagesDeltaCollectionPage.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
            {
                messagesDeltaCollectionPage.InitializeNextPageRequest(graphClient, deltaLink.ToString());
                messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();

            }
            //// Create a new message.
            //CreateNewMessage();

            //// Now let's use the deltalink to make sure there aren't any changes. We expect to see a new message.
            //if (messagesDeltaCollectionPage.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
            //{
            //    messagesDeltaCollectionPage.InitializeNextPageRequest(graphClient, deltaLink.ToString());
            //    messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();
            //}

            return new Dictionary<string, List<Message>> { { userId, messages } };
        }
    }
}
