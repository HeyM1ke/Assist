using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class UpdateData
    {
        public string updateUrl { get; set; }
        public string updateVersion { get; set; }
        public string updateChangelog { get; set; }
        public Mandatory mandatory { get; set; }

        public class Mandatory
        {
            public bool value { get; set; }
            public string minVersion { get; set; }
            public int mode { get; set; }
        }
    }
}
