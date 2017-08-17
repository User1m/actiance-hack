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
        public async static Task monitorMessages()
        {
            var messages = await APIService.GetMessageDeltasForUser(Storage.user.Id);

            foreach (KeyValuePair<string, List<Message>> entry in messages)
            {
                string currentUserId = entry.Key;
                entry.Value.ForEach(
                  async (x) =>
                {
                    if (x.BodyPreview.Contains(Resources.ResourceManager.GetString("DLPPhrase")))
                    {
                        Console.WriteLine("here");
                        var tasks = new List<Task>();
                        var members = await MessagesController.GetConverationMembers();
                        foreach (var member in members)
                        {
                            if (member.ObjectId == currentUserId)
                            {
                                tasks.Add(MessageUserAsync(member));
                            }
                        }
                        await Task.WhenAll(tasks);
                    }

                });

            }
        }

        public static async Task MessageUserAsync(TeamsChannelAccount user)
        {
            await MessagesController.MessageUser(user);
        }
    }
}
