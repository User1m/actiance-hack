using System.Collections.Generic;
using Newtonsoft.Json;

public class Value
{

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("businessPhones")]
    public IList<object> businessPhones { get; set; }

    [JsonProperty("displayName")]
    public string displayName { get; set; }

    [JsonProperty("givenName")]
    public string givenName { get; set; }

    [JsonProperty("jobTitle")]
    public object jobTitle { get; set; }

    [JsonProperty("mail")]
    public object mail { get; set; }

    [JsonProperty("mobilePhone")]
    public object mobilePhone { get; set; }

    [JsonProperty("officeLocation")]
    public object officeLocation { get; set; }

    [JsonProperty("preferredLanguage")]
    public string preferredLanguage { get; set; }

    [JsonProperty("surname")]
    public string surname { get; set; }

    [JsonProperty("userPrincipalName")]
    public string userPrincipalName { get; set; }
}

public class Example
{

    [JsonProperty("@odata.context")]
    public string odata_context { get; set; }

    [JsonProperty("value")]
    public IList<Value> value { get; set; }
}

