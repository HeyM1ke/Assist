using System;
using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi.Valorant
{

    public class Bundle
    {

        public Guid DataAssetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExtraDescription { get; set; }
        public string PromoDescription { get; set; }
        public string DisplayIcon { get; set; }
        public string VerticalPromoImage { get; set; }

    }
}
