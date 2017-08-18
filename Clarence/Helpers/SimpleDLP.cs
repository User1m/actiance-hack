using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Actiance.Helpers
{
    static class SimpleDLP
    {
        private static List<String> dlpKeywords = new List<string>()
        {
            "share",
            "short sell",
            "illegal",
            "ssn"
        };

        private static string ssnRegExStr = "[0-9]{3}-[0-9]{2}-[0-9]{4}";
        private static string creditCardRegExStr = "([0-9]{4}[- ]{0,1}){3,3}[0-9]{4}";

        private static List<Regex> regExDLPs = new List<Regex>()
        {
            new Regex(ssnRegExStr, RegexOptions.Compiled),
            new Regex(creditCardRegExStr, RegexOptions.Compiled)
        };

        public static bool containsRestrictedPhrases(string msg)
        {
            bool retVal = false;
            try
            {
                string msgInLowerCase = msg.ToLower();
                foreach (string keyword in dlpKeywords)
                {
                    retVal = msgInLowerCase.Contains(keyword);

                    if (retVal)
                    {
                        break;
                    }
                    else
                    {
                        foreach (Regex regex in regExDLPs)
                        {
                            retVal = regex.IsMatch(msgInLowerCase);
                            if (retVal)
                            {
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while checking for restricted phrases: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return retVal;
        }
    }
}
