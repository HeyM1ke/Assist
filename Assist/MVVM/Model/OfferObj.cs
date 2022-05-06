using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class OfferObj
    {

        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }

        [JsonPropertyName("isDirectPurchase")]
        public bool IsDirectPurchase { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("cost")]
        public OfferCost Cost { get; set; }

        [JsonPropertyName("rewards")]
        public List<Reward> Rewards { get; set; }

        public class OfferCost
        {
            [JsonPropertyName("vpCost")]
            public int VpCost { get; set; }
        }

        public class Reward
        {
            [JsonPropertyName("itemTypeID")]
            public string ItemTypeID { get; set; }

            [JsonPropertyName("itemID")]
            public string ItemID { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }
        }
    }
}
