using System.Collections.Generic;

// ReSharper disable All 

namespace Qianliyun_Launcher.API
{
    public class UserInfo
    {
        public long? Id { get; set; }
        public string Username { get; set; }
        public string Alias { get; set; }
        public string Reseller_alias { get; set; }
        public long? Auth_max { get; set; }
        public long? Auth_left { get; set; }
        public long? Deauth_left { get; set; }
        public long? Reseller { get; set; }
    }

    public class LiveSession
    {
        public long? Id { get; set; }
        public long? UserID { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Host { get; set; }
        public string Comment { get; set; }
        public float Begin { get; set; }
        public float End { get; set; }
        public List<string> Tags { get; set; }
    }

    public class CustomerInfo
    {
        public long? Id { get; set; }
        public long? User_id { get; set; }
        public string Customer_name { get; set; }
        public string Mobile { get; set; }
        public string Status { get; set; }
        public List<string> Tags { get; set; }
    }

    public class LiveViewer
    {
        public long? Id { get; set; }
        public long? User_id { get; set; }
        public long? Live_id { get; set; }
        public long? Customer_id { get; set; }
    }
}
