namespace Assist.Objects.AssistApi.Game;

public class CreateLobbyData
{
    public string lobbyName { get; set; }
    public string? region { get; set; }
    public bool isPrivate { get; set; }
    public bool requiresPassword { get; set; }
    public string password { get; set; }
    public string valorantPartyId { get; set; }
    public string code { get; set; }
}