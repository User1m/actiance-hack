using System;
using System.Collections.Generic;
using Microsoft.Graph;

namespace Actiance.Helpers
{
    public static class Storage
    {
        public static List<User> userStore { get; set; } = null;
        public static User manager { get; set; } = null;
        public static User user { get; set; } = null;
    }
}
