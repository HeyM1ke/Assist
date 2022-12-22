
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ValNet.Objects.ThirdParty.ValorantApi_VersionResp;

namespace Assist.Objects.RiotSocket
{
    internal class DefaultSocketDataMessage
    {
        public string eventType { get; set; }
        public string uri { get; set; }
        
    }
}
