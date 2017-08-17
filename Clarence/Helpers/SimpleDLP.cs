using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Actiance.Helpers
{
    class SimpleDLP
    {
        private List<String> dlpKeywords;
        private List<Regex> regExDLPs;
        private static string ssnRegExStr = "[0-9]{3}-[0-9]{2}-[0-9]{4}";
        private static string creditCardRegExStr = "([0-9]{4}[- ]{0,1}){3,3}[0-9]{4}";
        public SimpleDLP(List<String> keywords)
        {
            dlpKeywords = new List<string>();
            regExDLPs = new List<Regex>();

            foreach(string keyword in keywords)
            {
                if (!String.IsNullOrEmpty(keyword)) {
                    string keywordInLower = keyword.ToLower();
                    switch(keywordInLower)
                    {
                        case "ssn":
                            try
                            {
                                Regex re = new Regex(ssnRegExStr, RegexOptions.Compiled);
                                regExDLPs.Add(re);
                            } catch(Exception e)
                            {
                                Console.WriteLine("Error initializing ssn regular exp: " + e.Message);

                            }
                            break;
                            
                        case "creditcard":
                            try
                            {
                                Regex re = new Regex(creditCardRegExStr, RegexOptions.Compiled);
                                regExDLPs.Add(re);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error initializing credit card regular exp: " + e.Message);
                            }
                            break;
                        default:
                            dlpKeywords.Add(keywordInLower);
                            break;
                    }
                }
            }
        }

        public bool containsRestrictedPhrases(string msg)
        {
            bool retVal = false;
            try
            {
                string msgInLowerCase = msg.ToLower();
                foreach(string keyword in dlpKeywords)
                {
                    retVal = msgInLowerCase.Contains(keyword);

                    if(retVal)
                    {
                        break;
                    }
                    else
                    {
                        foreach(Regex regex in regExDLPs)
                        {
                            retVal = regex.IsMatch(msgInLowerCase);
                            if(retVal)
                            {
                                break;
                            }
                            
                        }
                    }
                }
            } catch(Exception e)
            {
                Console.WriteLine("Error while checking for restricted phrases: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return retVal;
        }
    }
}
