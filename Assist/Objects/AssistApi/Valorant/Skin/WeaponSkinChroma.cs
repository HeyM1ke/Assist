using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi.Valorant.Skin
{
    public class WeaponSkinChroma
    {
        [JsonPropertyName("chromaUuid")] public string Uuid { get; set; }

        public string DisplayName { get; set; }

        public string DisplayIcon { get; set; }

        public string StreamedVideoUrl { get; set; }
    }
}