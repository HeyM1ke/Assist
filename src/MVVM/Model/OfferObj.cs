using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    internal class OfferObj
    {

        public string offerID { get; set; }
        public bool isDirectPurchase { get; set; }
        public string startDate { get; set; }
        public Cost cost { get; set; }
        public List<Reward> rewards { get; set; }

        public class Cost
        {
            public int valorantPointCost { get; set; }
        }

        public class Reward
        {
            public string itemTypeID { get; set; }
            public string itemID { get; set; }
            public int quantity { get; set; }
        }
    }
}
