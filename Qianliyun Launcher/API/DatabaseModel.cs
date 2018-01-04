using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable All 

namespace Qianliyun_Launcher.API
{
    public class UserInfo
    {
        public Int64 id;
        public string username;
        public string alias;
        public string reseller_alias;
        public int auth_max;
        public int auth_left;
        public int deauth_left;
        public Int64 reseller;
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
