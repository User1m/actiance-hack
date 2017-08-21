using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Graph;
using System.Linq;
using Chronic;
using Actiance.App_LocalResources;
using Microsoft.Bot.Connector.Teams.Models;
using System.Threading.Tasks;
using Actiance.Helpers;
//using System.Threading;
//using System.Timers;
using System.Threading;


namespace Actiance.Services
{
    public static class MonitorService
    {

        public static TeamsChannelAccount[] members;

        public async static Task<bool> IngestMessagesForUser(string userId)
        {
            if (Storage.userMessages.ContainsKey(userId))
            {
                var tasks = new List<Task>();
                foreach (Message entry in Storage.userMessages[userId])
                {
                    if (SimpleDLP.containsRestrictedPhrases(entry.BodyPreview))
                    {
                        if (entry.Sender.EmailAddress.Name != "Clarence")
                        {
                            Console.WriteLine("-------------\nFOUND ISSUE\n-------------");
                            string recipientsEmails = string.Empty;
                            foreach (var rep in entry.ToRecipients)
                            {
                                recipientsEmails += $"{rep.EmailAddress.Address},";
                            }
                            tasks.Add(SendComplianceMsgAsync(Storage.userStore[userId][Storage.teamsInfo] as TeamsChannelAccount, entry.BodyPreview, entry.Sender.EmailAddress.Address, recipientsEmails));
                            break;
                        }
                    }

                    ///message user once
                    if (tasks.Count > 0) { break; }
                }
                await Task.WhenAll(tasks);
            }
            return true;
        }

        public static async Task SendComplianceMsgAsync(TeamsChannelAccount user, string msg, string senderEmail, string recipientsEmails)
        {
            await MessagesController.MessageUsers(user, msg, senderEmail, recipientsEmails);
        }

        public static async Task Monitor(string objectId)
        {
            try
            {
                Console.WriteLine($"-------------\nSTARTED INGESTING USER: {objectId} MESSAGES\n-------------");

                //foreach (var objectId in Storage.userStore.Keys)
                //{
                List<Message> userMessages = await APIService.GetInitialMessageDeltasForUser(objectId);

                //if (Storage.userMessages[objectId] == null)
                Storage.userMessages.Add(objectId, userMessages);
                //else
                //Storage.userMessages[objectId] = userMessages;

                await IngestMessagesForUser(objectId);
                //}

                Console.WriteLine($"-------------\nDONE INGESTING USER: {objectId} MESSAGES\n-------------");


                // Thread thread = new Thread(() =>
                //{
                //    System.Timers.Timer t = new System.Timers.Timer();
                //    t.AutoReset = false;
                //    t.Interval = 10 * 1000;

                //    Thread.CurrentThread.IsBackground = true;
                //    Console.WriteLine($"-------------\nSTARTED TIMER FOR USER: {objectId}\n-------------");
                //    t.Elapsed += async delegate
                //    {
                //        try
                //        {
                //            await PollForUserMessages();
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine($"ERROR IN TIMER THREAD: {e.Message}");
                //            Console.WriteLine($"ERROR IN TIMER THREAD: {e.StackTrace}");
                //        }
                //        finally
                //        {
                //            t.Start();
                //        }
                //    };
                //    t.Start();
                //});
                //thread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR IN BACKGROUND THREAD-0: {e.Message}");
                Console.WriteLine($"ERROR IN BACKGROUND THREAD-0: {e.StackTrace}");
            }
        }

        public static async Task<bool> PollForUserMessages()
        {
            Console.WriteLine("-------------\nTIMER: Polling Again \n-------------");

            try
            {
                if (Storage.deltaStore.Count > 0)
                {
                    await APIService.GetUpdatedMessageFromDeltaLink();
                    foreach (var objectId in Storage.userStore.Keys)
                    {
                        await IngestMessagesForUser(objectId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR IN BACKGROUND THREAD-1: {e.Message}");
                Console.WriteLine($"ERROR IN BACKGROUND THREAD-1: {e.StackTrace}");
            }
            return true;
        }
    }
}
