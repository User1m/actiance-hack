﻿namespace GraphWebhooks.Controllers
{
    public class SubscriptionService
    {

        public string Endpoint { get; set; } = "https://graph.microsoft.com/v1.0/subscriptions/";
        public string Id { get; set; }

        public void CreateSubscription()
        {
            // Get Token

            // Subscribe to Messages, OneDrive Items, Teams stuff.
        }
        
    }
}