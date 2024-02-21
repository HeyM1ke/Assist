namespace Assist.Objects.AssistApi.Game;

public class RequestPartyJoin
{
    public string PartyId { get; set; }
    public bool IsPrivate { get; set; } = false;
    public string CurrentGameName { get; set; }
    public string CurrentTag { get; set; }
}