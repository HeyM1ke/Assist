namespace Assist.Objects.Helpers;

public class QueueNames
{
    public static string DetermineQueueKey(string queueId)
    {
        switch (queueId)
        {
            case "ggteam":
                return "Escalation";
            case "deathmatch":
                return "Deathmatch";
            case "spikerush":
                return "SpikeRush";
            case "competitive":
                return "Competitive";
            case "unrated":
                return "Unrated";
            case "onefa":
                return "Replication";
            case "swiftplay":
                return "Swiftplay";
            case "snowball":
                return "Snowball";
            case "lotus":
                return "Lotus";
            case "newmap":
                return "Sunset";
            case "premier-seasonmatch":
                return "Premier";
            case "hurm":
                return "Team Deathmatch";
            default:
                return "VALORANT";
        }

    }
}