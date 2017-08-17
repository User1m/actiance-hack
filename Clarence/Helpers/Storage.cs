using System;
using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;

namespace Actiance.Helpers
{
    public static class Storage
    {
        public static List<User> userStore { get; set; } = null;
        public static User manager { get; set; } = null;
        public static User user { get; set; } = null;
        public static Activity activity { get; set; }
        public static IDialogContext context { get; set; }
        public static IDictionary<string, List<MessageDeltaCollectionResponse>> userMessages { get; set; } = new Dictionary<string, List<MessageDeltaCollectionResponse>>();
    }
}
