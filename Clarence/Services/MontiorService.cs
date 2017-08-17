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
        public async static Task<bool> MonitorMessages()
        {
            var messages = await APIService.GetMessageDeltasForUser(Storage.user.Id);

            foreach (KeyValuePair<string, List<Message>> entry in messages)
            {
                var tasks = new List<Task>();
                string currentUserId = entry.Key;
                entry.Value.ForEach(
                async (x) =>
                {
                    if (x.BodyPreview.Contains(Resources.ResourceManager.GetString("DLPPhrase")))
                    {
                        Console.WriteLine("here");
                        var members = await MessagesController.GetConverationMembers();
                        foreach (var member in members)
                        {
                            if (member.ObjectId == currentUserId)
                            {
                                //message user once
                                tasks.Add(SendComplianceMsgAsync(member, x.BodyPreview));
                                break;
                            }
                        }
                    }
                });
                await Task.WhenAll(tasks);
            }
            return true;
        }

        public static async Task SendComplianceMsgAsync(TeamsChannelAccount user, string msg)
        {
            await MessagesController.MessageUserAndManager(user, msg);
        }
    }
}
