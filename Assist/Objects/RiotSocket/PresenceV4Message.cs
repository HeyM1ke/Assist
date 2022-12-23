using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.Objects.RiotSocket
{
    internal class PresenceV4Message
    {
        public Data data { get; set; }
        public string eventType { get; set; }
        public string uri { get; set; }

        public class Data
        {
            public Presence[] presences { get; set; }
        }

        public class Presence
        {
            public object actor { get; set; }
            public string basic { get; set; }
            public string details { get; set; }
            public string game_name { get; set; }
            public string game_tag { get; set; }
            public string location { get; set; }
            public string msg { get; set; }
            public string name { get; set; }
            public string patchline { get; set; }
            public string pid { get; set; }
            public object platform { get; set; }

            [JsonPropertyName("private")]
            public string Private { get; set; }

            public object privateJwt { get; set; }
            public string product { get; set; }
            public string puuid { get; set; }
            public string region { get; set; }
            public string resource { get; set; }
            public string state { get; set; }
            public string summary { get; set; }
            public long time { get; set; }
        }
    }
}