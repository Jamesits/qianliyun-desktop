using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Qianliyun_Launcher.BroadcastCapture
{
    public class Capture
    {
        public String GUID;
        public String name;
        public String URL;
    }

    public class CaptureResultEntry
    {
        public String username;
        public String userAction;
        public String content;
        public DateTime time;
    }

    public class CaptureResultStorage
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Capture captureProperties;
        public List<CaptureResultEntry> resultEntries;

        public CaptureResultStorage(string guid, string name, string url)
        {
            captureProperties = new Capture(){ GUID = guid, name = name, URL = url};
            resultEntries = new List<CaptureResultEntry>();
            logger.Debug("Initialized capture result storage for GUID {0}, name {1}", guid, name);
        }

        public void addEntry(string username, string useraction)
        {
            var content = "";
            if (username.EndsWith(":"))
            {
                username = username.TrimEnd(':');
                content = useraction;
                useraction = "发言";
            }
            resultEntries.Add(new CaptureResultEntry(){username = username, userAction = useraction, content = content, time = DateTime.Now});
            logger.Debug("New record: {0} {1}", username, useraction);
        }
    }
}
