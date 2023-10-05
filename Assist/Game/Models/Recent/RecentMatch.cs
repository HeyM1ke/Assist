using System;
using System.Collections.Generic;

namespace Assist.Game.Models.Recent;

public class RecentMatch
{
    public string MatchId { get; set; } = String.Empty;
    public bool IsCompleted { get; set; } = false;
    public MatchResult Result { get; set; } = MatchResult.DEFAULT;
    public string MapId { get; set; } = String.Empty;
    public string Gamemode { get; set; } = String.Empty;
    public string QueueId { get; set; } = string.Empty;
    public DateTime DateOfMatch { get; set; }
    public double LengthOfMatchInSeconds { get; set; }
    public double AllyTeamScore { get; set; } = 0;
    public double EnemyTeamScore { get; set; } = 0;
    public string AllyTeamName { get; set; } = String.Empty;
    public string AllyTeamId { get; set; } = String.Empty;
    public string EnemyTeamName { get; set; } = String.Empty;
    public string MatchTrack_LastState { get; set; } = string.Empty;
    public string OwningPlayer { get; set; } = string.Empty;

    public List<Player> Players { get; set; } = new List<Player>();
    

    public class Player
    {
        public string PlayerId { get; set; } = String.Empty;
        public string PlayerName { get; set; } = "Player";
        public string PlayerTag { get; set; } = "NA1";
        public string PlayerAgentId { get; set; } = "0";
        public Stats? Statistics { get; set; } = new ();
        public int CompetitiveTier { get; set; } = 0;
        public string TeamId { get; set; } = string.Empty;
        public class Stats
        {
            public int Kills { get; set; } = 0;
            public int Deaths { get; set; } = 0;
            public int Assists { get; set; } = 0;
            public int Score { get; set; } = 0;
        }
        
    }
    
    public enum MatchResult
    {
        DEFAULT,
        REMAKE,
        IN_PROGRESS,
        VICTORY,
        LOSS
    }
}