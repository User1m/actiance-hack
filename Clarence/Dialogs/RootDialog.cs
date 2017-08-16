using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Actiance.App_LocalResources;
using System.Globalization;
using System.Resources;
using System.Reflection;
using Actiance.Services;
using Actiance.Helpers;

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
            //get uset provide
            string contextName = context.Activity.From.Name;
            if (Storage.user != null)
            {
                await APIService.GetUser(contextName);
                await APIService.GetManager(contextName);
            }

            string userName = (string.IsNullOrEmpty(Storage.user.GivenName) ? context.Activity.From.Name : Storage.user.GivenName);
            string welcomeMsg = string.Format(CultureInfo.InvariantCulture, Resources.ResourceManager.GetString("Welcome"), userName);
            await context.PostAsync(welcomeMsg);

            //context.Wait(MessageReceivedAsync);
        }

        //private static Attachment GetSigninCard()
        //{
        //    var signinCard = new SigninCard
        //    {
        //        Text = "BotFramework Sign-in Card",
        //        Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") }
        //    };

        //    return signinCard.ToAttachment();
        //}
    }
}