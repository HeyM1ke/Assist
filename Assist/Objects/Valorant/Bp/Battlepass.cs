using System.Text.Json.Serialization;

namespace Assist.Objects.Valorant.Bp
;

public class Battlepass
{

    [JsonPropertyName("battlepassName")]
    public string Name { get; set; }

    public string Id { get; set; }
    public string DisplayIcon { get; set; }
    public BattlepassChapter[] Chapters { get; set; }

}
