using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Assist.Objects.AssistApi.Game;

public class AssistLobby
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonProperty("lobbyName")]
    [JsonPropertyName("lobbyName")]
    public string LobbyName { get; set; }

    [JsonProperty("region")]
    [JsonPropertyName("region")]
    public string Region { get; set; }

    [JsonProperty("requiresPassword")]
    [JsonPropertyName("requiresPassword")]
    public bool RequiresPassword { get; set; }

    [JsonProperty("currentPartySize")]
    [JsonPropertyName("currentPartySize")]
    public int CurrentPartySize { get; set; }

    [JsonProperty("maxPartySize")]
    [JsonPropertyName("maxPartySize")]
    public int MaxPartySize { get; set; }

    [JsonProperty("code")]
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonProperty("createdAt")]
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("isExpired")]
    [JsonPropertyName("isExpired")]
    public bool IsExpired { get; set; }
}