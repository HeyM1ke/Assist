using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Assist.Objects.AssistApi.Game;

public class UpdateLobbyData
{
    [JsonProperty("currentPartySize")]
    [JsonPropertyName("currentPartySize")]
    public int CurrentPartySize { get; set; }

    [JsonProperty("maxPartySize")]
    [JsonPropertyName("maxPartySize")]
    public int MaxPartySize { get; set; }

    [JsonProperty("partyClosed")]
    [JsonPropertyName("partyClosed")]
    public bool PartyClosed { get; set; }

    [JsonProperty("valorantIdsParty")]
    [JsonPropertyName("valorantIdsParty")]
    public List<string> ValorantIdsParty { get; set; }

    [JsonProperty("currentValorantQueue")]
    [JsonPropertyName("currentValorantQueue")]
    public string CurrentValorantQueue { get; set; }
}