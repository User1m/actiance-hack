using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Actiance.App_LocalResources;

namespace Actiance.Dialogs
{
    [Serializable]
    public class SupportDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Resources.ResourceManager.GetString("HelpMessage"));
            context.Done<object>(null);
            //context.Wait(this.MessageReceivedAsync);
        }

        //public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        //{
        //    var message = await result;
        //    await context.PostAsync(Resources.ResourceManager.GetString("HelpMessage"));
        //    context.Done<object>(null);
        //}
    }
}
