using System;
using System.Collections.Generic;
using Actiance.Models;
using Newtonsoft.Json;

namespace Actiance.Models
{
    public class Body
    {

        [JsonProperty("contentType")]
        public string contentType { get; set; }

        [JsonProperty("content")]
        public string content { get; set; }
    }

    public class EmailAddress
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }
    }

    public class Sender
    {

        [JsonProperty("emailAddress")]
        public EmailAddress emailAddress { get; set; }
    }

    public class From
    {

        [JsonProperty("emailAddress")]
        public EmailAddress emailAddress { get; set; }
    }

    public class Flag
    {

        [JsonProperty("flagStatus")]
        public string flagStatus { get; set; }
    }

    public class Value
    {

        [JsonProperty("@odata.type")]
        public string odata_type { get; set; }

        [JsonProperty("@odata.etag")]
        public string odata_etag { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime createdDateTime { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public DateTime lastModifiedDateTime { get; set; }

        [JsonProperty("changeKey")]
        public string changeKey { get; set; }

        [JsonProperty("categories")]
        public IList<object> categories { get; set; }

        [JsonProperty("receivedDateTime")]
        public DateTime receivedDateTime { get; set; }

        [JsonProperty("sentDateTime")]
        public DateTime sentDateTime { get; set; }

        [JsonProperty("hasAttachments")]
        public bool hasAttachments { get; set; }

        [JsonProperty("internetMessageId")]
        public string internetMessageId { get; set; }

        [JsonProperty("subject")]
        public string subject { get; set; }

        [JsonProperty("bodyPreview")]
        public string bodyPreview { get; set; }

        [JsonProperty("importance")]
        public string importance { get; set; }

        [JsonProperty("parentFolderId")]
        public string parentFolderId { get; set; }

        [JsonProperty("conversationId")]
        public string conversationId { get; set; }

        [JsonProperty("conversationIndex")]
        public string conversationIndex { get; set; }

        [JsonProperty("isDeliveryReceiptRequested")]
        public bool isDeliveryReceiptRequested { get; set; }

        [JsonProperty("isReadReceiptRequested")]
        public bool isReadReceiptRequested { get; set; }

        [JsonProperty("isRead")]
        public bool isRead { get; set; }

        [JsonProperty("isDraft")]
        public bool isDraft { get; set; }

        [JsonProperty("webLink")]
        public string webLink { get; set; }

        [JsonProperty("inferenceClassification")]
        public string inferenceClassification { get; set; }

        [JsonProperty("unsubscribeData")]
        public IList<object> unsubscribeData { get; set; }

        [JsonProperty("unsubscribeEnabled")]
        public bool unsubscribeEnabled { get; set; }

        [JsonProperty("body")]
        public Body body { get; set; }

        [JsonProperty("sender")]
        public Sender sender { get; set; }

        [JsonProperty("from")]
        public From from { get; set; }

        [JsonProperty("toRecipients")]
        public IList<object> toRecipients { get; set; }

        [JsonProperty("ccRecipients")]
        public IList<object> ccRecipients { get; set; }

        [JsonProperty("bccRecipients")]
        public IList<object> bccRecipients { get; set; }

        [JsonProperty("replyTo")]
        public IList<object> replyTo { get; set; }

        [JsonProperty("mentionsPreview")]
        public object mentionsPreview { get; set; }

        [JsonProperty("flag")]
        public Flag flag { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }
    }

    public class MessageDeltaQueryResponse
    {

        [JsonProperty("@odata.context")]
        public string odata_context { get; set; }

        [JsonProperty("@odata.nextLink")]
        public string odata_nextLink { get; set; }

        [JsonProperty("value")]
        public IList<Value> value { get; set; }
    }

}
