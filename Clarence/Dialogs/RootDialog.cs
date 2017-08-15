using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;

namespace Actiance.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //var activity = await result as Activity;
            //ConfigurationManager.AppSettings["ServiceUrl"] = activity.ServiceUrl;
            //await context.PostAsync($"I just cached your service URL {activity.ServiceUrl}");
            await context.PostAsync($"HI there!");


            context.Wait(MessageReceivedAsync);
        }
    }
}