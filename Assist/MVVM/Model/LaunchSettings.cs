using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class LaunchSettings
    {
        public string ValPatchline { get; set; } = "live";
        public bool ValDscRpcEnabled { get; set; } = true;  
        public bool EnableCustomParams { get; set; }
        public string CustomValParams { get; set; }
    }
}
