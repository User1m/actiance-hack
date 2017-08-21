using System;
using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector.Teams.Models;

namespace Actiance.Helpers
{
    public static class Storage
    {
        public static string selfInfo = "selfInfo";
        public static string managerInfo = "managerInfo";
        public static string teamsInfo = "teamsInfo";

        public static Dictionary<string, Dictionary<string, object>> userStore { get; set; } = new Dictionary<string, Dictionary<string, object>>();
        public static Activity activity { get; set; } = null;
        public static IDialogContext context { get; set; } = null;
        public static Dictionary<string, IMessageDeltaCollectionPage> deltaStore { get; set; } = new Dictionary<string, IMessageDeltaCollectionPage>();
        public static Dictionary<string, List<Message>> userMessages { get; set; } = new Dictionary<string, List<Message>>();
    }
}
