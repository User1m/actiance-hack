using System;
using Newtonsoft.Json;

namespace Actiance.Models
{

    public class InnerError
    {

        [JsonProperty("request-id")]
        public string request_id { get; set; }

        [JsonProperty("date")]
        public DateTime date { get; set; }
    }

    public class Error
    {

        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("innerError")]
        public InnerError innerError { get; set; }
    }

    public class TokenError
    {

        [JsonProperty("error")]
        public Error error { get; set; }
        public TokenError() { }
    }
}
