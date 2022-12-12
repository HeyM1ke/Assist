using System.Text.Json.Serialization;

namespace Assist.Objects.AssistApi.Valorant.Battlepass
{
    public class Battlepass
    {
        [JsonPropertyName("battlepassName")] public string Name { get; set; }

        public string Id { get; set; }
        public string DisplayIcon { get; set; }
        public BattlepassChapter[] Chapters { get; set; }
    }

    public class BattlepassChapter
    {
        public bool IsEpilogue { get; set; }
        public BattlepassLevel[] Levels { get; set; }
    }

    public class BattlepassLevel
    {
        public string RewardName { get; set; }
        public string RewardId { get; set; }
        public string RewardDisplayIcon { get; set; }
        public int RequiredXp { get; set; }
        public BattlepassReward Reward { get; set; }

        [JsonPropertyName("vpCost")] public int Cost { get; set; }

        [JsonPropertyName("purchasableWithVp")]
        public bool PurchasableWithPoints { get; set; }
    }

    public class BattlepassReward
    {
        public string Type { get; set; }
        public object TitleText { get; set; }
        public string PlayercardLargeArt { get; set; }
        public string SprayFullImage { get; set; }
    }
}