using System;
using System.Text.Json.Serialization;

namespace Assist.Objects.Valorant;

public class Bundle
{

    public Guid DataAssetId { get; init; }

    [JsonPropertyName("bundleName")]
    public string Name { get; init; }
    public string Description { get; init; }
    public string ExtraDescription { get; init; }
    public string PromoDescription { get; init; }
    public string DisplayIcon { get; init; }
    public string VerticalPromoImage { get; init; }

}
