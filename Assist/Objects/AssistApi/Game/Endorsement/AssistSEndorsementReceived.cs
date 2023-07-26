namespace Assist.Objects.AssistApi.Game.Endorsement;

public class AssistSEndorsementReceived
{
    public string PlayerId{ get; set; } // Who is receiving the message.
    public string UsernameOfGiver{ get; set; } // Username of who gave it.
}