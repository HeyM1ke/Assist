using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Services.Riot
{
    internal class ClientSettings
    {
        public string Patchline { get; set; } = "live";
        public bool EnableCustomParams { get; set; }
        public string CustomValParams { get; set; }
    }
}
