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
            var tasks = new List<Task>();

            foreach (Message entry in Storage.userMessages[userId])
            {
                //if (entry.BodyPreview.Contains(Resources.ResourceManager.GetString("DLPPhrase")))
                if (SimpleDLP.containsRestrictedPhrases(entry.BodyPreview))
                {
                    if (members == null)
                    {
                        members = await MessagesController.GetConverationMembers();
                    }
                    foreach (var member in members)
                    {
                        if (member.ObjectId == userId && entry.Sender.EmailAddress.Name != "Clarence")
                        {
                            Console.WriteLine("-------------\nFOUND ISSUE\n-------------");
                            string recipientsEmails = string.Empty;
                            foreach (var rep in entry.ToRecipients)
                            {
                                recipientsEmails += $"{rep.EmailAddress.Address},";
                            }
                            tasks.Add(SendComplianceMsgAsync(member, entry.BodyPreview, entry.Sender.EmailAddress.Address, recipientsEmails));
                            break;
                        }
                    }
                }

                ///message user once
                if (tasks.Count > 0) { break; }
            }
            await Task.WhenAll(tasks);
            return true;
        }

        public static async Task SendComplianceMsgAsync(TeamsChannelAccount user, string msg, string senderEmail, string recipientsEmails)
        {
            await MessagesController.MessageUserAndManager(user, msg, senderEmail, recipientsEmails);
        }

        public static async Task Monitor()
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.AutoReset = false;
            t.Interval = 10 * 1000;

            try
            {
                if (members == null)
                {
                    members = await MessagesController.GetConverationMembers();
                }

                Console.WriteLine("-------------\n INGESTING ALL USER MESSAGES \n-------------");

                foreach (var member in members)
                {
                    List<Message> userMessages = await APIService.GetInitialMessageDeltasForUser(member.ObjectId);
                    Storage.userMessages.Add(member.ObjectId, userMessages);
                    await IngestMessagesForUser(member.ObjectId);
                }

                t.Elapsed += async delegate
                {
                    try
                    {
                        await PollForUserMessages();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR IN TIMER THREAD: {e.Message}");
                        Console.WriteLine($"ERROR IN TIMER THREAD: {e.StackTrace}");
                    }
                    finally
                    {
                        t.Start();
                    }
                };
                t.Start();
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
                    foreach (var member in members)
                    {
                        await IngestMessagesForUser(member.ObjectId);
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
