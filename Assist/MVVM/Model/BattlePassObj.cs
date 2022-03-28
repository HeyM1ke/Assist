using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class BattlePassObj
    {




        public List<RewardItem> itemsInChapter { get; set; }
        public bool isEpilogue { get; set; }

        public List<FreeRewardItem> freeItemsInChapter { get; set; }

        public class RewardItem
        {
            public string rewardName { get; set; }
            public int xP_Required { get; set; }
            public bool isPurchasableWithVP { get; set; }
            public int vpCost { get; set; }

            public int tierNumber { get; set; }
            public string rewardId { get; set; }
            public string imageUrl { get; set; }

            public ExtraData extraData { get; set; }
        }

        public class FreeRewardItem
        {
            public string RewardName { get; set; }
            public string RewardId { get; set; }
            public string ImageUrl { get; set; }
        }

        public class ExtraData
        {
            public string rewardType { get; set; }
            public string title_Text { get; set; }
            public string playercard_LargeArt { get; set; }
            public string spray_FullImage { get; set; }
        }
    }


    public class BattlepassComplete
    {
        public List<BattlePassObj> Chapters;
    }
}
