using System.Collections.Generic;

// ReSharper disable All 

namespace Qianliyun_Launcher.API
{
#pragma warning disable IDE1006 // Naming Styles
    public class UserInfo
    {

        public long? id { get; set; }
        public string username { get; set; }
        public string alias { get; set; }
        public string reseller_alias { get; set; }
        public long? auth_max { get; set; }
        public long? auth_left { get; set; }
        public long? deauth_left { get; set; }
        public long? reseller { get; set; }
    }

    public class LiveSession
    {
        public long? id { get; set; }
        public long? user_id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string host { get; set; }
        public string comment { get; set; }
        public float begin { get; set; }
        public float end { get; set; }
        public List<string> tags { get; set; }
    }

    public class CustomerInfo
    {
        public long? id { get; set; }
        public long? user_id { get; set; }
        public string customer_name { get; set; }
        public string mobile { get; set; }
        public string status { get; set; }
        public List<string> tags { get; set; }
    }

    public class LiveViewer
    {
        public long? id { get; set; }
        public long? user_id { get; set; }
        public long? live_id { get; set; }
        public long? customer_id { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
