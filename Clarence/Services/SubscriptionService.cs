/*
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license.
 *  See LICENSE in the source repository root for complete license information.
 */


namespace GraphWebhooks.Controllers
{
    public class SubscriptionService
    {

        public string Endpoint { get; set; } = "https://graph.microsoft.com/v1.0/subscriptions/";
        public string Id { get; set; }


        public void CreateSubscription()
        {
        }
        
        public void DeleteSubscription()
        {
        }
    }
}