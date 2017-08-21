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
using Microsoft.Graph;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;
using System.Linq;

namespace Actiance
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private static ConnectorClient connector;
        //private Timer timer;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<IHttpActionResult> Post([FromBody]Activity activity)
        {
            Storage.activity = activity;
            connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            var members = await GetConverationMembers();
            try
            {
                foreach (var member in members)
                {
                    if (!Storage.userStore.ContainsKey(member.ObjectId))
                        Storage.userStore.Add(member.ObjectId, new Dictionary<string, object> { { Storage.teamsInfo, member } });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR IN USERSTORE: {e.Message}");
                Console.WriteLine($"ERROR IN USERSTORE: {e.StackTrace}");
            }


            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.AsMessageActivity().Text.Contains("/clear"))
                {
                    await _reset();
                }

                else
                {
                    await Microsoft.Bot.Builder.Dialogs.Conversation.SendAsync(activity, () => new MainDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }

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
            /// Fetch the members in the current conversation
            var convoId = Storage.activity.Conversation.Id;
            var tenantId = Storage.activity.GetTenantId();
            var members = await connector.Conversations.GetTeamsConversationMembersAsync(convoId, tenantId);
            return members;
        }

        public static async Task EmailUsers(string userFullName, string flaggedMsg, List<Recipient> recipients, string managerId)
        {
            ///email manager
            Message email = new Message
            {
                Body = new ItemBody
                {
                    Content = string.Format(Resources.ResourceManager.GetString("EmailContent"), userFullName, flaggedMsg),
                    ContentType = BodyType.Text,
                },
                Subject = Resources.ResourceManager.GetString("EmailSubject"),
                ToRecipients = recipients
            };

            // Send the message.
            await APIService.graphv1Client.Users[managerId].SendMail(email, true).Request().PostAsync();
        }

        public static async Task MessageUsers(string msg, string senderEmail, string recipientsEmails)
        {
            var recipientsEmailsArray = recipientsEmails.Split(',').Where(x => !string.IsNullOrEmpty(x));
            var recipients = new List<Recipient> {
                new Recipient{
                    EmailAddress = new EmailAddress{
                        Address = senderEmail
                    }
                },

            };
            foreach (var email in recipientsEmailsArray)
            {
                recipients.Add(
                 new Recipient
                 {
                     EmailAddress = new EmailAddress
                     {
                         Address = email
                     }
                 });
            }

            foreach (var userItem in Storage.userStore.ToList())
            {
                string userEmail = (userItem.Value[Storage.teamsInfo] as TeamsChannelAccount).UserPrincipalName;

                if (recipientsEmailsArray.Contains(userEmail))
                {
                    // Create or get existing chat conversation with user
                    TeamsChannelAccount userTeamsInfo = userItem.Value[Storage.teamsInfo] as TeamsChannelAccount;
                    var response = connector.Conversations.CreateOrGetDirectConversation(Storage.activity.Recipient, userTeamsInfo, Storage.activity.GetTenantId());

                    User manager = userItem.Value[Storage.managerInfo] as User;
                    msg = (string.IsNullOrEmpty(msg)) ? msg : $"\"{msg}\"";
                    var resourceString = Resources.ResourceManager.GetString("ComplianceMessage");
                    var responseMsg = string.Format(resourceString, msg, userTeamsInfo.GivenName, manager.GivenName);
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

                    recipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = manager.Mail
                        }
                    });

                    await EmailUsers($"{userTeamsInfo.GivenName} {userTeamsInfo.Surname}", msg, recipients, manager.Id);

                    // Post the message to chat conversation with user
                    await connector.Conversations.SendToConversationAsync(newMessage, response.Id);
                }
            }
        }
    }
}