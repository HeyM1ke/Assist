using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class BattlePassObj
    {
        public string id { get; set; }
        public object displayIcon { get; set; }
        public string battlepassName { get; set; }
        public List<Chapter> chapters { get; set; }



        public class Reward
        {
            public string type { get; set; }
            public object titleText { get; set; }
            public string playercardLargeArt { get; set; }
            public string sprayFullImage { get; set; }
        }

        public class Level
        {
            public string rewardName { get; set; }
            public string rewardId { get; set; }
            public string rewardDisplayIcon { get; set; }
            public int requiredXp { get; set; }
            public int vpCost { get; set; }
            public bool purchasableWithVp { get; set; }
            public Reward reward { get; set; }
        }

        public class Chapter
        {
            public bool isEpilogue { get; set; }
            public List<Level> levels { get; set; }
        }
    }
}
