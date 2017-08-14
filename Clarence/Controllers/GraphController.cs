using Actiance.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Http;

namespace Actiance.Controllers
{
    public class GraphController : ApiController
    {
        Func<Notification, IMessageActivity> AsMessage = (note) => new Activity(id: note.SubscriptionId, type: note.ChangeType, value: note);
        
        public void Mail([FromBody] IEnumerable<Notification> notes)
        {
            notes.Select(async note => await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog()));
        }

        public void Calendar([FromBody] IEnumerable<Notification> notes)
        {
            notes.Select(async note => await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog()));
        }

        public void GroupConversations([FromBody] IEnumerable<Notification> notes)
        {
            notes.Select(async note => await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog()));
        }

        public void DriveRootItems([FromBody] IEnumerable<Notification> notes)
        {
            notes.Select(async note => await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog()));
        }
    }
}