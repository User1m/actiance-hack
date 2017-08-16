using System;
namespace Actiance.Helpers
{

    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;

#pragma warning disable 1998

    public class Middleware : IActivityLogger
    {
        public async Task LogAsync(IActivity activity)
        {
            Console.WriteLine($"From ID: {activity.From.Id}\nTo ID: {activity.Recipient.Id}\nMessage:\n------------\n{activity.AsMessageActivity()?.Text}\n------------");
        }
    }


}
