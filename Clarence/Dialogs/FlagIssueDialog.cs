using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.Interface;
using Actiance.App_LocalResources;

namespace Actiance.Dialogs
{
    [Serializable]
    public class FlagIssueDialog : IAppDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context.Activity.AsMessageActivity().Text.Equals("Flag something"))
            {
                await TypeAndMessage(context, Resources.ResourceManager.GetString("FlagMain"));
            }
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            string msg = message.Text.ToLower();
            string response = "";
            string[] responses = {
                "Thanks it has been flagged!",
                "It shall be done. Thank you.",
                "Your wish is my commmand"
            };

            bool continueDialog = false;

            if (msg.Equals("Flag something"))
            {
                continueDialog = true;
                response = Resources.ResourceManager.GetString("FlagMain");
            }
            else if (!msg.Contains("flag:"))
            {
                response = Resources.ResourceManager.GetString("FlagError");
            }
            else
            {
                response = responses[new Random().Next(0, responses.Length - 1)];
            }

            //await MessagesController.SendTyping(context);
            //await context.PostAsync(response);
            await TypeAndMessage(context, response);

            if (continueDialog)
            {
                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                context.Done(true);
            }
        }

        public async Task TypeAndMessage(IDialogContext context, string response)
        {
            await MessagesController.SendTyping();
            await context.PostAsync(response);
        }
    }
}
