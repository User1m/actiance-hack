using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Actiance.Controllers;
using Actiance.Helpers;
using System.Linq;

namespace Actiance.Services
{
    public static class APIService
    {

        public static string graphv1Endpoint = $"{AuthController.msGraph}/v1.0";
        public static string graphBetaEndpoint = $"{AuthController.msGraph}/beta";

        public static GraphServiceClient graphBetaClient = new GraphServiceClient(
            graphBetaEndpoint, new AzureAuthenticationProvider()
               //new DelegateAuthenticationProvider(
               //async (requestMessage) =>
               //{
               //    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await AuthController.GetOauthToken());

               //    return Task.FromResult(0);
               //})
               );
        public static GraphServiceClient graphv1Client = new GraphServiceClient(
            graphv1Endpoint, new AzureAuthenticationProvider()
        );


        public static async Task<TResult> GetFrom<TResult>(string url, string token)
        {
            ///get from https://graph.microsoft.com/
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(url);
                return await response.Content.ReadAsAsync<TResult>();
            }
        }

        public static async Task<TResult> PostTo<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            ///post to https://login.microsoftonline.com/db35d98a-b61b-4362-90e6-22237a243507/oauth2/v2.0/token

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

        public static async Task<List<User>> GetAllAADTenantUsers()
        {
            //Storage.userStore = await GetFrom<List<User>>($"{graphv1Endpoint}/users/", await AuthController.GetOauthToken());
            List<User> users = await graphv1Client.Users.Request().GetAsync() as List<User>;
            return users;
        }

        public static async Task<User> GetUserProfile(string userName)
        {
            //User user = await GetFrom<User>($"{graphv1Endpoint}/users/{userName}@{AuthController.tenant}", await AuthController.GetOauthToken());
            User user = await graphv1Client.Users[$"{userName}@{AuthController.tenant}"].Request().GetAsync();
            return user;
        }

        public static async Task<User> GetUserManager(string userId)
        {
            //User manager = await GetFrom<User>($"{graphv1Endpoint}/users/{userId}/Manager/", await AuthController.GetOauthToken());
            User manager = await graphv1Client.Users[userId].Manager.Request().GetAsync() as User;
            return manager;
        }

        public static async Task<bool> GetUpdatedMessageFromDeltaLink()
        {
            //var localStore = Storage.deltaStore;
            foreach (KeyValuePair<string, IMessageDeltaCollectionPage> query in Storage.deltaStore.ToList())
            {
                var page = query.Value;
                List<Message> messages = new List<Message>();

                while (page.NextPageRequest != null)
                {
                    page = await page.NextPageRequest.GetAsync();
                    foreach (var message in page)
                    {
                        messages.Add(message);
                    }
                }

                object deltaLink;
                if (page.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
                {
                    page.InitializeNextPageRequest(graphBetaClient, deltaLink.ToString());
                    page = await page.NextPageRequest.GetAsync();
                    foreach (var message in page)
                    {
                        messages.Add(message);
                    }
                }

                //update messages & deltas
                Storage.userMessages[query.Key] = messages;
                Storage.deltaStore[query.Key] = page;
            }

            return true;
        }

        public static async Task<List<Message>> GetInitialMessageDeltasForUser(string userId)
        {

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
            /// Get our first delta page.
            //string filter = $"receivedDateTime ge {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")}";
            //var messagesDeltaCollectionPage = await graphBetaClient.Users[userId].MailFolders["teamchat"].Messages.Delta().Request().Filter(filter).GetAsync();
            var messagesDeltaCollectionPage = await graphBetaClient.Users[userId].MailFolders["teamchat"].Messages.Delta().Request().GetAsync();

            /// Go through all of the delta pages so that we can get the delta link on the last page.
            while (messagesDeltaCollectionPage.NextPageRequest != null)
            {
                messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();
                foreach (var message in messagesDeltaCollectionPage)
                {
                    messages.Add(message);
                }
            }

            /// At this point we're up to date. messagesDeltaCollectionPage now has a deltalink.  
            object deltaLink;

            /// Now let's use the deltalink to make sure there aren't any changes. There shouldn't be.
            if (messagesDeltaCollectionPage.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
            {
                messagesDeltaCollectionPage.InitializeNextPageRequest(graphBetaClient, deltaLink.ToString());
                messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();
                foreach (var message in messagesDeltaCollectionPage)
                {
                    messages.Add(message);
                }
            }

            Storage.deltaStore.Add(userId, messagesDeltaCollectionPage);

            //// Now let's use the deltalink to make sure there aren't any changes. We expect to see a new message.
            //if (messagesDeltaCollectionPage.AdditionalData.TryGetValue("@odata.deltaLink", out deltaLink))
            //{
            //    messagesDeltaCollectionPage.InitializeNextPageRequest(graphClient, deltaLink.ToString());
            //    messagesDeltaCollectionPage = await messagesDeltaCollectionPage.NextPageRequest.GetAsync();
            //}

            return messages;
        }
    }
}
