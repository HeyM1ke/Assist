using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Objects.AssistApi.Server
{
    internal class SendServerChatMessage
    {
        public string UserId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime TimeSent { get; set; } = DateTime.Now;
    }
}
