using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime time;
    }

    public class CaptureStorage
    {
        public List<CaptureResultEntry> CaptureResults;
        public CaptureStorage()
        {
            CaptureResults = new List<CaptureResultEntry>();
        }

        public bool newCaptureResult()
        {
            return true;
        }
    }
}
