using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class AssistBundleObj
    {
        [JsonPropertyName("dataAssetId")]
        public Guid DataAssetId { get; init; }

        [JsonPropertyName("bundleName")]
        public string BundleName { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; }

        [JsonPropertyName("extraDescription")]
        public string ExtraDescription { get; init; }

        [JsonPropertyName("promoDescription")]
        public string PromoDescription { get; init; }

        [JsonPropertyName("displayIcon")]
        public string DisplayIcon { get; init; }

        [JsonPropertyName("verticalPromoImage")]
        public string VerticalPromoImage { get; init; }
    }
}
