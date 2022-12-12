using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi.Valorant.Offer
{
    public class OfferCost
    {
        [JsonPropertyName("vpCost")] public int Cost { get; set; }
    }
}