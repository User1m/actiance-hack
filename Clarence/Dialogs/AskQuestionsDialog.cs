using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Actiance.Dialogs
{
    [Serializable]
    public class AskQuestionsDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"HI there!");

            //context.Wait(MessageReceivedAsync);
        }
    }
}
