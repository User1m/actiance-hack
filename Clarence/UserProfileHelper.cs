using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading.Tasks;
using Actiance.Models;

namespace Actiance
{
    public class UserProfileHelper
    {
        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public User GetUserProfile(String accessToken, String userPrincipalName)
        {
            //post to https://login.microsoftonline.com/db35d98a-b61b-4362-90e6-22237a243507/oauth2/v2.0/token
            string url = "https://graph.microsoft.com/v1.0/users/" + userPrincipalName;

            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Authorization"] = "Bearer " + accessToken;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
            settings.UseSimpleDictionaryFormat = true;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(User), settings);
            //m_Logger.Info("Entering response stream not null");
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string res = sr.ReadToEnd();
                    
                    using (Stream stream = GenerateStreamFromString(res))
                    {
                        Object objResponse = jsonSerializer.ReadObject(stream);
                        
                        return (User)objResponse;
                    }
                }
            }
        }
    }
}