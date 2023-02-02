using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Assist.Objects.AssistApi.Game;

public class CreateLobbyResp
{
    [JsonProperty("isSuccessful")]
    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; set; }

    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonProperty("lobby")]
    [JsonPropertyName("lobby")]
    public AssistLobby Lobby { get; set; }
}