using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class AssistNewsObj
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("newsTitle")]
        public string NewsTitle { get; set; }

        [JsonPropertyName("newsDescription")]
        public string NewsDescription { get; set; }

        [JsonPropertyName("newsImage")]
        public string NewsImage { get; set; }

        [JsonPropertyName("newsUrl")]
        public string NewsUrl { get; set; }
    }
}
