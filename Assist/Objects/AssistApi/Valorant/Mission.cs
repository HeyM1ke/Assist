using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.Objects.AssistApi.Valorant
{
    internal class Mission
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("xpGrant")]
        public int XpGrant { get; set; }

        [JsonPropertyName("progressToComplete")]
        public int ProgressToComplete { get; set; }

        [JsonPropertyName("activationDate")]
        public DateTime ActivationDate { get; set; }

        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }
    }
}
