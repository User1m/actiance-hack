using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Actiance.Controllers;
using System.Globalization;
using System.Configuration;
using Actiance.Models;

namespace Actiance.Services
{
    public class APIService
    {
        public APIService() { }

        private static string graphEndpoint = string.Format("{0}/{1}", AuthController.msGraph, "v1.0");

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

        public static async void GetUsers()
        {
            Storage.userStore = await GetFrom<List<User>>(string.Format("{0}/users/", graphEndpoint), AuthController.GetOauthToken().ToString());
        }

        public static async void GetManager(string user)
        {
            User users = await GetFrom<User>(string.Format("{0}/users/{1}/Manager/", graphEndpoint, user), AuthController.GetOauthToken().ToString());
        }
    }
}
