using Actiance.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Actiance.Controllers
{
    public class GraphController : ApiController
    {
        Func<Notification, IMessageActivity> AsMessage = (note) => new Activity(id: note.SubscriptionId, type: note.ChangeType, value: note);
        
        public async void Mail([FromBody] Notification note)
        {
            await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog());
        }

        public async void Calendar([FromBody] Notification note)
        {
            await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog());
        }

        public async void GroupConversations([FromBody] Notification note)
        {
            await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog());
        }

        public async void DriveRootItems([FromBody] Notification note)
        {
            await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog());
        }


        // The `notificationUrl` endpoint that's registered with the webhook subscription.
        [HttpPost]
        public async Task<ActionResult> Listen()
        {

            // Validate the new subscription by sending the token back to Microsoft Graph.
            // This response is required for each subscription.
            if (Request.QueryString["validationToken"] != null)
            {
                var token = Request.QueryString["validationToken"];
                return Content(token, "plain/text");
            }

            // Parse the received notifications.
            else
            {
                try
                {
                    var notifications = new Dictionary<string, Notification>();
                    using (var inputStream = new System.IO.StreamReader(Request.InputStream))
                    {
                        JObject jsonObject = JObject.Parse(inputStream.ReadToEnd());
                        if (jsonObject != null)
                        {

                            // Notifications are sent in a 'value' array. The array might contain multiple notifications for events that are
                            // registered for the same notification endpoint, and that occur within a short timespan.
                            JArray value = JArray.Parse(jsonObject["value"].ToString());
                            foreach (var notification in value)
                            {
                                Notification current = JsonConvert.DeserializeObject<Notification>(notification.ToString());

                                // Check client state to verify the message is from Microsoft Graph. 
                                SubscriptionStore subscription = SubscriptionStore.GetSubscriptionInfo(current.SubscriptionId);

                                // This sample only works with subscriptions that are still cached.
                                if (subscription != null)
                                {
                                    if (current.ClientState == subscription.ClientState)
                                    {

                                        // Just keep the latest notification for each resource.
                                        // No point pulling data more than once.
                                        notifications[current.Resource] = current;
                                    }
                                }
                            }

                            if (notifications.Count > 0)
                            {

                                // Query for the changed messages. 
                                await GetChangedMessagesAsync(notifications.Values);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    // TODO: Handle the exception.
                    // Still return a 202 so the service doesn't resend the notification.
                }
                return new HttpStatusCodeResult(202);
            }
        }

    }
}