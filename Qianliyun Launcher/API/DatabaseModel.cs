using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

// ReSharper disable All 

namespace Qianliyun_Launcher.API
{
    public class UserInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("reseller_alias")]
        public string ResellerAlias { get; set; }

        [JsonProperty("auth_max")]
        public long? AuthMax { get; set; }

        [JsonProperty("auth_left")]
        public long? AuthLeft { get; set; }

        [JsonProperty("deauth_left")]
        public long? DeauthLeft { get; set; }

        [JsonProperty("reseller")]
        public long? Reseller { get; set; }
    }

    public class LiveSession : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("begin")]
        public float? Begin { get; set; }

        [JsonProperty("end")]
        public float? End { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }

    public class CustomerInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }

    public class LiveViewer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("live_id")]
        public long? LiveId { get; set; }

        [JsonProperty("customer_id")]
        public long? CustomerId { get; set; }
    }
}
