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
    public class MainDialog : IDialog<object>
    {
        private const string AskQestion = "Ask a question";
        private const string FlagContent = "Flag something";

        private const string HelpCMD = "HELP:";
        private const string AskCMD = "ASK:";
        private const string FlagCMD = "FLAG:";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            string contextName = context.Activity.From.Name;
            //if (Storage.user != null)
            //{
            //    //get user profie
            //    await APIService.GetUser(contextName);
            //    await APIService.GetManager(contextName);
            //}

            var message = await result;

            string msg = message.Text.ToLower();
            string cmd = null;

            if (msg.Contains("help") || msg.Contains("support") || msg.Contains("problem"))
            {
                cmd = HelpCMD;
            }
            else if (msg.Contains("ask") || msg.Contains("question") || msg.Contains("ask:") || msg.Contains("question:"))
            {
                cmd = AskCMD;
            }
            else if (msg.Contains("flag") || msg.Contains("flag:") || msg.Contains("item"))
            {
                cmd = FlagCMD;
            }

            switch (cmd)
            {
                case HelpCMD:
                    await context.Forward(new SupportDialog(), this.ResumeAfterDoneDialog, message, CancellationToken.None);
                    break;
                case AskCMD:
                    await context.Forward(new AskQuestionsDialog(), this.ResumeAfterDoneDialog, message, CancellationToken.None);
                    break;
                case FlagCMD:
                    await context.Forward(new FlagIssueDialog(), this.ResumeAfterDoneDialog, message, CancellationToken.None);
                    break;
                default:
                    this.ShowOptions(context);
                    break;
            }

        }

        private void ShowOptions(IDialogContext context)
        {
            string userName = (Storage.user == null ? context.Activity.From.Name : Storage.user.GivenName);
            string welcomeMsg = string.Format(CultureInfo.InvariantCulture, Resources.ResourceManager.GetString("Welcome"), userName);
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { AskQestion, FlagContent }, welcomeMsg, "Not a valid option", 1);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case AskQestion:
                        await context.Forward(new AskQuestionsDialog(), this.ResumeAfterDoneDialog, optionSelected, CancellationToken.None);
                        break;

                    case FlagContent:
                        await context.Forward(new FlagIssueDialog(), this.ResumeAfterDoneDialog, optionSelected, CancellationToken.None);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                Console.WriteLine(ex.Message);
                await context.PostAsync($"Were you looking to do something else? Please Try Again.");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                //var message = await result;
                //await context.PostAsync($"Thanks for contacting me");

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message:\n-----------\n{ex.Message}\n-----------");

            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private Task ResumeAfterDoneDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(true);
            return Task.CompletedTask;
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