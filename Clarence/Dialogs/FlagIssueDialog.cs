using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Actiance.Dialogs
{
    [Serializable]
    public class FlagIssueDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context.Activity.AsMessageActivity().Text.Equals("Flag something"))
            {
                await context.PostAsync("What non-compliance related message would you like to flag?");
            }
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var message = await result;

            string msg = message.Text.ToLower();
            string response = "";
            string[] responses = {
                "Thanks it has been flagged!",
                "It shall be done",
                "Your wish is my commmand"
            };

            bool continueDialog = false;

            if (msg.Equals("Flag something"))
            {
                continueDialog = true;
                response = "What non-compliance related message would you like to flag?";
            }
            else if (!msg.Contains("flag:"))
            {
                response = "Please phrase you question in this way - flag: <Issue>";
            }
            else
            {
                response = responses[new Random().Next(0, responses.Length - 1)];
            }

            await context.PostAsync(response);
            if (continueDialog)
            {
                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                context.Done<object>(null);
            }
        }
    }
}
