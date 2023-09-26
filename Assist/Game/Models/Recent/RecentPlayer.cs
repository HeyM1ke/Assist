using System;
using System.Collections.Generic;

namespace Assist.Game.Models.Recent;

public class RecentPlayer
{
    public string PlayerId { get; set; }
    public List<string> Matches { get; set; } = new();
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public string LastSeenMatchId { get; set; } = string.Empty;
}