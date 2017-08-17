using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.Interface;
using Actiance.App_LocalResources;

namespace Actiance.Dialogs
{
    [Serializable]
    public class AskQuestionsDialog : IAppDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context.Activity.AsMessageActivity().Text.Equals("Ask a question"))
            {
                await TypeAndMessage(context, Resources.ResourceManager.GetString("AskMain"));
            }

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            string msg = message.Text.ToLower();
            string response = "";
            string[] responses = {
                "NO!",
                "I think we both know the answer is NO...",
                "Why would you want to do that?",
                "Ask HR",
                "The answer is not clear. Try again later..."
            };
            bool continueDialog = false;

            if (msg.Equals("Ask a question"))
            {
                continueDialog = true;
                response = Resources.ResourceManager.GetString("AskMain");
            }
            else if (!msg.Contains("ask:"))
            {
                continueDialog = true;
                response = Resources.ResourceManager.GetString("AskError");
            }
            else if (msg.Contains("ssn"))
            {
                response = Resources.ResourceManager.GetString("AskSSN");
            }
            else
            {
                response = responses[new Random().Next(0, responses.Length - 1)];
            }

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
