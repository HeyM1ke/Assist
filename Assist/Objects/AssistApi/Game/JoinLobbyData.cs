using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Assist.Objects.AssistApi.Game;

public class JoinLobbyData
{
    [JsonProperty("isSuccessful")]
    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; set; }

    [JsonProperty("partyId")]
    [JsonPropertyName("partyId")]
    public string PartyId { get; set; }

    [JsonProperty("isPrivate")]
    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonProperty("partyClosed")]
    [JsonPropertyName("partyClosed")]
    public bool PartyClosed { get; set; }

    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; }
}