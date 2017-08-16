using Actiance.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;


namespace Actiance.Controllers
{
    public class GraphController : ApiController
    {
        public class NotificationEvent
        {
            public IEnumerable<Notification> Value { get; set; }
        }

        Func<Notification, IMessageActivity> AsMessage = (note) => new Activity(id: note.SubscriptionId, type: note.ChangeType, value: note);

        /// <summary>
        /// Need to return the token for accepting webhooks
        /// </summary>
        /// <param name="validationToken"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromUri] string validationToken)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(validationToken, Encoding.UTF8, "text/plain") };
        }

        public IHttpActionResult Post([FromBody] NotificationEvent notifications)
        {
            notifications.Value.Select(async note => await Conversation.SendAsync(AsMessage(note), () => new Dialogs.NotificationDialog()));
            return Content(HttpStatusCode.Accepted, "");
        }
    }
}