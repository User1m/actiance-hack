using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.Models;

namespace Actiance.Dialogs
{
    [Serializable]
    public class NotificationDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var notification = activity.Value as Notification;

            var connector = new ConnectorClient(new Uri(Config.ServiceUrl));
            
            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = Config.Bot;
            message.Conversation = new ConversationAccount(true, "conversation");
            message.ChannelId = Config.ChannelId;

            message.Locale = "en-En";
            message.Text = $"You made a change to the file {notification.Resource}";
            
            connector.Conversations.SendToConversation((Activity)message);

            context.Wait(MessageReceivedAsync);
        }
    }
}