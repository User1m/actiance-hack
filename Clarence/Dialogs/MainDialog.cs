using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Actiance.App_LocalResources;
using System.Globalization;
using Actiance.Services;
using Actiance.Helpers;
using System.Threading;
using Actiance.Interface;

namespace Actiance.Dialogs
{

    [Serializable]
    public class MainDialog : IAppDialog<object>
    {
        private const string AskQestion = "Ask a question";
        private const string FlagContent = "Flag something";

        private const string HelpCMD = "HELP:";
        private const string AskCMD = "ASK:";
        private const string FlagCMD = "FLAG:";
        private const string GratitudeCMD = "THANKS:";
        private const string DebugCMD = "DEBUG:";



        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Storage.context = context;
            string contextName = context.Activity.From.Name;
            if (Storage.user == null)
            {
                //call API services
                Storage.user = await APIService.GetUserProfile(contextName);
                Storage.manager = await APIService.GetUserManager(Storage.user.Id);

                //start monitoring service on background thread
                //await MontiorService.IngestMessagesForUser(Storage.user.Id);
                new Thread(async () =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    try
                    {
                        await MontiorService.IngestMessagesForUser(Storage.user.Id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR IN BACKGROUND THREAD: {e.Message}");
                        Console.WriteLine($"ERROR IN BACKGROUND THREAD: {e.StackTrace}");
                    }
                }).Start();
            }

            var message = await result;

            string msg = message.Text.ToLower();
            string cmd = null;

            if (msg.Contains("help") || msg.Contains("support") || msg.Contains("problem") || msg.Contains("what can you do"))
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
            else if (msg.Contains("thank") || msg.Equals("ok"))
            {
                cmd = GratitudeCMD;
            }
            else if (msg.Contains("debug") || msg.Equals("dev"))
            {
                cmd = DebugCMD;
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
                case GratitudeCMD:
                    await TypeAndMessage(context, Resources.ResourceManager.GetString("Welcomed"));
                    break;
                case DebugCMD:
                    await TypeAndMessage(context, $"{Storage.user.GivenName}'s Manager is {Storage.manager.GivenName} at {DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssZ")}");
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
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { AskQestion, FlagContent }, welcomeMsg, Resources.ResourceManager.GetString("InvalidOption"), 1);
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
                await TypeAndMessage(context, string.Format(Resources.ResourceManager.GetString("OptionError"), (Storage.user == null ? context.Activity.From.Name : Storage.user.GivenName)));
                context.Wait(this.MessageReceivedAsync);
            }
        }

        //private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        //{
        //    try
        //    {
        //        //var message = await result;
        //        //await context.PostAsync($"Thanks for contacting me");
        //    }
        //    catch (Exception ex)
        //    {
        //        await context.PostAsync($"Failed with message:\n-----------\n{ex.Message}\n-----------");
        //    }
        //    finally
        //    {
        //        context.Wait(this.MessageReceivedAsync);
        //    }
        //}

        private Task ResumeAfterDoneDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(true);
            return Task.CompletedTask;
        }

        public async Task TypeAndMessage(IDialogContext context, string response)
        {
            await MessagesController.SendTyping();
            await context.PostAsync(response);
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