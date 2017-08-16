using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.Dialogs;

namespace Actiance
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<IHttpActionResult> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
                await Conversation.SendAsync(activity, () => new RootDialog());
            else
                HandleSystemMessage(activity);
            return Ok();
        }

        private Activity HandleSystemMessage(Activity message)
        {
            switch(message.Type)
            {
                case ActivityTypes.DeleteUserData: break; // Implement user deletion here if we handle user deletion, return a real message
                case ActivityTypes.ConversationUpdate: break; // Handle state changes like member adding/removing
                case ActivityTypes.ContactRelationUpdate: break; // add/remove from contact lists
                case ActivityTypes.Typing: break; // Handle knowing user is typing
                case ActivityTypes.Ping: break;
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
    }
}