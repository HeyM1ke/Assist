using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Objects.AssistApi.Server
{
    internal class ServerChatMessage
    {
        public string Username { get; set; } = string.Empty;
        public List<string> BadgeIds { get; set; } = new List<string>();
        public DateTime TimeSent { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
    }
}
