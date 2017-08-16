using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Actiance.Dialogs
{
    [Serializable]
    public class AskQuestionsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context.Activity.AsMessageActivity().Text.Equals("Ask a question"))
            {
                await context.PostAsync("What compliance related question would you like to ask?");
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
                response = "What compliance related question would you like to ask?";
            }
            else if (!msg.Contains("ask:"))
            {
                continueDialog = true;
                response = "Please phrase you question in this way - ask: <Question>";
            }
            else if (msg.Contains("ssn"))
            {
                response = "NO! It's never wise to share Peronally Identifiable Information (PII) such as you SSN in public domain.";
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
