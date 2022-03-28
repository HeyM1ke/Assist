using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class SkinObj
    {

        public string uuid { get; set; }
        public string displayName { get; set; }
        public string themeUuid { get; set; }
        public string displayIcon { get; set; }

        public List<Chroma> chromas { get; set; }

        public List<Level> levels { get; set; }

        public class Level
        {
            public string LevelUuid { get; set; }
            public string displayName { get; set; }
            public string displayIcon { get; set; }
            public string streamedVideoUrl { get; set; }
        }

        public class Chroma
        {
            public string ChromaUuid { get; set; }
            public string displayName { get; set; }
            public string displayIcon { get; set; }
            public string streamedVideoUrl { get; set; }

        }
    }
}
