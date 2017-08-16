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
using System.Threading;

namespace Actiance.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string AskQestion = "Ask a question";
        private const string FlagContent = "Flag something";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string contextName = context.Activity.From.Name;
            if (Storage.user != null)
            {
                //get user profie
                await APIService.GetUser(contextName);
                await APIService.GetManager(contextName);
            }


            var message = await result;

            string msg = message.Text.ToLower();

            if (msg.Contains("help") || msg.Contains("support") || msg.Contains("problem"))
            {
                context.Call(new SupportDialog(), this.ResumeAfterSupportDialog);
            }
            else if (msg.Contains("ask") || msg.Contains("question"))
            {
                context.Call(new AskQuestionsDialog(), this.ResumeAfterOptionDialog);
            }
            else if (msg.Equals("hello") || msg.Contains("hi"))
            {
                this.ShowOptions(context);
            }
            else
            {
                await context.PostAsync("Say *help* or ask: *<question>* or flag: *<issue>*");
                context.Wait(MessageReceivedAsync);
            }

        }

        private void ShowOptions(IDialogContext context)
        {
            string userName = (Storage.user == null ? context.Activity.From.Name : Storage.user.GivenName);
            string welcomeMsg = string.Format(CultureInfo.InvariantCulture, Resources.ResourceManager.GetString("Welcome"), userName);
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { AskQestion, FlagContent }, welcomeMsg, "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case AskQestion:
                        context.Call(new AskQuestionsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case FlagContent:
                        context.Call(new FlagIssueDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                Console.WriteLine(ex.Message);
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                //var message = await result;
                await context.PostAsync($"Thanks for contacting me");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"What would you like to do?");
            context.Done<object>(null);
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