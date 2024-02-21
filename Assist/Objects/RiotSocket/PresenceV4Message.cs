using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ValNet.Objects.Local;

namespace Assist.Objects.RiotSocket
{
    public class PresenceV4Message
    {
        public Data data { get; set; }
        public string eventType { get; set; }
        public string uri { get; set; }

        public class Data
        {
            public List<ChatV4PresenceObj.Presence> presences { get; set; }
        }
    }
}