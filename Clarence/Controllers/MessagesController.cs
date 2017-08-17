using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.Dialogs;
using Actiance.Helpers;
using System;
using Actiance.Services;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Actiance.App_LocalResources;

namespace Actiance
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private static ConnectorClient connector;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<IHttpActionResult> Post([FromBody]Activity activity)
        {
            Storage.activity = activity;
            connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.AsMessageActivity().Text.Contains("/clear"))
                {
                    await _reset();
                }

                else
                {
                    await Conversation.SendAsync(activity, () => new MainDialog());
                }
            }
            else
                HandleSystemMessage(activity);
            return Ok();
        }

        private Activity HandleSystemMessage(Activity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData: break; // Implement user deletion here if we handle user deletion, return a real message
                case ActivityTypes.ConversationUpdate: break; // Handle state changes like member adding/removing
                case ActivityTypes.ContactRelationUpdate: break; // add/remove from contact lists
                case ActivityTypes.Typing: break; // Handle knowing user is typing
                case ActivityTypes.Ping: break;
            }

            return null;
        }

        private async Task _reset()
        {
            Activity activity = Storage.activity;
            await activity.GetStateClient().BotState
                .DeleteStateForUserWithHttpMessagesAsync(activity.ChannelId, activity.From.Id);

            var clearMsg = activity.CreateReply();
            clearMsg.Text = $"Reseting everything for conversation: {activity.Conversation.Id}";
            await connector.Conversations.SendToConversationAsync(clearMsg);
        }

        public static async Task SendTyping()
        {
            Activity activity = Storage.activity;
            Activity reply = activity.CreateReply();
            reply.Type = ActivityTypes.Typing;
            reply.Text = null;
            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        public static async Task<TeamsChannelAccount[]> GetConverationMembers()
        {
            // Fetch the members in the current conversation
            var convoId = Storage.activity.Conversation.Id;
            var tenantId = Storage.activity.GetTenantId();
            var members = await connector.Conversations.GetTeamsConversationMembersAsync(convoId, tenantId);
            return members;
        }

        public static async Task MessageUserAndManager(TeamsChannelAccount user, string msg = "")
        {
            // Create or get existing chat conversation with user
            var response = connector.Conversations.CreateOrGetDirectConversation(Storage.activity.Recipient, user, Storage.activity.GetTenantId());

            Microsoft.Graph.User manager = Storage.manager;
            if (Storage.user.Id != user.ObjectId)
            {
                manager = await APIService.GetManager(user.ObjectId);
            }
            msg = (string.IsNullOrEmpty(msg)) ? msg : $"\"{msg}\"";
            var resourceString = Resources.ResourceManager.GetString("ComplianceMessage");
            var responseMsg = string.Format(resourceString, msg, user.GivenName, manager.GivenName);
            // Construct the message to post to conversation
            Activity newMessage = new Activity()
            {
                Text = responseMsg,
                Type = ActivityTypes.Message,
                Conversation = new ConversationAccount
                {
                    Id = response.Id
                },
            };

            // Post the message to chat conversation with user
            await connector.Conversations.SendToConversationAsync(newMessage, response.Id);
        }
    }
}