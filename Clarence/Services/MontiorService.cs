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

namespace Actiance.Services
{
    public static class MontiorService
    {
        public async static Task<bool> IngestMessagesForUser(string userId)
        {
            List<Message> userMessages = await APIService.GetMessageDeltasForUser(userId);
            Storage.userMessages.Add(userId, userMessages);
            var tasks = new List<Task>();

            foreach (Message entry in userMessages)
            {
                //if (entry.BodyPreview.Contains(Resources.ResourceManager.GetString("DLPPhrase")))
                if (SimpleDLP.containsRestrictedPhrases(entry.BodyPreview))
                {
                    Console.WriteLine("here");
                    var members = await MessagesController.GetConverationMembers();
                    foreach (var member in members)
                    {
                        if (member.ObjectId == userId)
                        {
                            tasks.Add(SendComplianceMsgAsync(member, entry.BodyPreview));
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

        public static async Task SendComplianceMsgAsync(TeamsChannelAccount user, string msg)
        {
            await MessagesController.MessageUserAndManager(user, msg);
        }
    }
}
