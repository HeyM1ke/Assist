using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi.Valorant.Offer
{
    public class StoreOffer
    {
        [JsonPropertyName("offerId")] public string Id { get; set; }

        public bool IsDirectPurchase { get; set; }

        public string StartDate { get; set; }

        public OfferCost Cost { get; set; }

        public OfferReward[] Rewards { get; set; }
    }
}