using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class UpdateData
    {
        public string version { get; init; }
        public string url { get; init; }
        public string changelog { get; init; }
        public MandatoryData mandatory { get; init; }

        public class MandatoryData
        {
            public bool value { get; init; }
            public string minVersion { get; init; }
            public int mode { get; init; }
        }
    }
}
