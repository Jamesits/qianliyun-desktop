using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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
        public Int64 id;
        public Int64 UserID;
        public string url;
        public string title;
        public string host;
        public string comment;
        public float begin;
        public float end;
        public List<string> tags;
    }

    public class CustomerInfo
    {
        public Int64 id;
        public Int64 user_id;
        public string customer_name;
        public string mobile;
        public string status;
        public List<string> tags;
    }

    public class LiveViewer
    {
        public Int64 id;
        public Int64 user_id;
        public Int64 live_id;
        public Int64 customer_id;
    }
}
