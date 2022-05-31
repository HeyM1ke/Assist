using System.Text.Json.Serialization;

namespace Assist.Objects.Valorant.Bp;

public class BattlepassLevel
{

    public string RewardName { get; set; }
    public string RewardId { get; set; }
    public string RewardDisplayIcon { get; set; }
    public int RequiredXp { get; set; }
    public BattlepassReward Reward { get; set; }

    [JsonPropertyName("vpCost")]
    public int Cost { get; set; }

    [JsonPropertyName("purchasableWithVp")]
    public bool PurchasableWithPoints { get; set; }

}
