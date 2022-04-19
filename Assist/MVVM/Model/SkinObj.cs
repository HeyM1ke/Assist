using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class AssistSkin
    {
        public class Chroma
        {
            [JsonPropertyName("chromaUuid")]
            public string ChromaUuid { get; set; }

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }

            [JsonPropertyName("displayIcon")]
            public string DisplayIcon { get; set; }

            [JsonPropertyName("streamedVideoUrl")]
            public string StreamedVideoUrl { get; set; }
        }

        public class Level
        {
            [JsonPropertyName("levelUuid")]
            public string LevelUuid { get; set; }

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }

            [JsonPropertyName("displayIcon")]
            public string DisplayIcon { get; set; }

            [JsonPropertyName("streamedVideoUrl")]
            public string StreamedVideoUrl { get; set; }
        }
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("themeUuid")]
        public string ThemeUuid { get; set; }

        [JsonPropertyName("displayIcon")]
        public string DisplayIcon { get; set; }

        [JsonPropertyName("chromas")]
        public List<Chroma> Chromas { get; set; }

        [JsonPropertyName("levels")]
        public List<Level> Levels { get; set; }

    }
}
