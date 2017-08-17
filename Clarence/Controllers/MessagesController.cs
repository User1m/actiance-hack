﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using Actiance.Dialogs;
using Actiance.Helpers;

namespace Actiance
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        //[HttpGet]
        //public string Get(string test = "dafault")
        //{
        //    return test;
        //}

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Storage.activity = activity;
            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.AsMessageActivity().Text.Contains("/clear"))
                {
                    await _reset(activity);
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new MainDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private async Task _reset(Activity activity)
        {
            await activity.GetStateClient().BotState
                .DeleteStateForUserWithHttpMessagesAsync(activity.ChannelId, activity.From.Id);

            var client = new ConnectorClient(new Uri(activity.ServiceUrl));
            var clearMsg = activity.CreateReply();
            clearMsg.Text = $"Reseting everything for conversation: {activity.Conversation.Id}";
            await client.Conversations.SendToConversationAsync(clearMsg);
        }

        public static async Task SendTyping(IDialogContext context)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(context.Activity.AsMessageActivity().ServiceUrl));
            Activity reply = Storage.activity.CreateReply();
            reply.Type = ActivityTypes.Typing;
            reply.Text = null;
            await connector.Conversations.ReplyToActivityAsync(reply);
        }
    }
}