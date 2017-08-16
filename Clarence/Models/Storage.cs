using System;
using System.Collections.Generic;
using Microsoft.Graph;

namespace Actiance.Models
{
    public static class Storage
    {
        public static List<User> userStore { get; set; }
        public static User manager { get; set; }

    }
}
