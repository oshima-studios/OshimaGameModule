using System.Text.Json.Serialization;

namespace Oshima.FunGame.WebAPI.Models
{
    public class Payload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("op")]
        public int Op { get; set; } = 0;

        [JsonPropertyName("d")]
        public object Data { get; set; } = new();

        [JsonPropertyName("s")]
        public int SequenceNumber { get; set; } = 0;

        [JsonPropertyName("t")]
        public string EventType { get; set; } = "";
    }

    public class ValidationRequest
    {
        [JsonPropertyName("plain_token")]
        public string PlainToken { get; set; } = "";

        [JsonPropertyName("event_ts")]
        public string EventTs { get; set; } = "";
    }

    public class ValidationResponse
    {
        [JsonPropertyName("plain_token")]
        public string PlainToken { get; set; } = "";

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = "";
    }

    public class BotConfig
    {
        public string AppId { get; set; } = "";
        public string Secret { get; set; } = "";
    }

    public class Author
    {
        [JsonPropertyName("user_openid")]
        public string UserOpenId { get; set; } = "";

        [JsonPropertyName("member_openid")]
        public string MemberOpenId { get; set; } = "";
    }

    public class Attachment
    {
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "";

        [JsonPropertyName("filename")]
        public string Filename { get; set; } = "";

        [JsonPropertyName("height")]
        public int Height { get; set; } = 0;

        [JsonPropertyName("width")]
        public int Width { get; set; } = 0;

        [JsonPropertyName("size")]
        public int Size { get; set; } = 0;

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
    }

    public interface IBotMessage
    {
        public string Id { get; }
        public bool IsGroup { get; }
        public string Detail { get; set; }
        public string Timestamp { get; }
        public string OpenId { get; }
        public string AuthorOpenId { get; }
        public long FunGameUID { get; set; }
        public bool UseNotice { get; set; }
    }

    public class ThirdPartyMessage : IBotMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("isgroup")]
        public bool IsGroup { get; set; } = false;

        [JsonPropertyName("group_openid")]
        public string GroupOpenId { get; set; } = "";

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("openid")]
        public string OpenId { get; set; } = "";

        [JsonPropertyName("authoropenid")]
        public string AuthorOpenId { get; set; } = "";

        [JsonIgnore]
        public bool IsCompleted { get; set; } = false;

        [JsonIgnore]
        public string Result { get; set; } = "";

        [JsonIgnore]
        public long FunGameUID { get; set; } = 0;

        [JsonIgnore]
        public bool UseNotice { get; set; } = true;
    }

    public class C2CMessage : IBotMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("author")]
        public Author Author { get; set; } = new();

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("attachments")]
        public Attachment[] Attachments { get; set; } = [];

        [JsonIgnore]
        public long FunGameUID { get; set; } = 0;

        [JsonIgnore]
        public bool UseNotice { get; set; } = true;

        public string Detail
        {
            get => Content;
            set => Content = value;
        }
        public string OpenId => Author.UserOpenId;
        public bool IsGroup => false;
        public string AuthorOpenId => Author.UserOpenId;
    }

    public class GroupAtMessage : IBotMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("author")]
        public Author Author { get; set; } = new();

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";

        [JsonPropertyName("group_openid")]
        public string GroupOpenId { get; set; } = "";

        [JsonPropertyName("attachments")]
        public Attachment[] Attachments { get; set; } = [];

        [JsonIgnore]
        public long FunGameUID { get; set; } = 0;

        [JsonIgnore]
        public bool UseNotice { get; set; } = true;

        public string Detail
        {
            get => Content;
            set => Content = value;
        }
        public string OpenId => GroupOpenId;
        public bool IsGroup => true;
        public string AuthorOpenId => Author.MemberOpenId;
    }

    public class MediaResponse
    {
        [JsonPropertyName("file_uuid")]
        public string FileUuid { get; set; } = "";

        [JsonPropertyName("file_info")]
        public string FileInfo { get; set; } = "";

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }
    }

    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; } = "";
    }
}
