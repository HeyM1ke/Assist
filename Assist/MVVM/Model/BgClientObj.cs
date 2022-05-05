using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class BgClientObj
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("versionNumber")]
        public string VersionNumber { get; set; }

        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
