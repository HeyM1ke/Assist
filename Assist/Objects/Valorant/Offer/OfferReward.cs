using System.Text.Json.Serialization;

namespace Assist.Objects.Valorant.Offer;

public class OfferReward
{

    [JsonPropertyName("itemTypeID")]
    public string ItemTypeId { get; set; }

    [JsonPropertyName("itemID")]
    public string ItemId { get; set; }

    public int Quantity { get; set; }

}
