using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Game.Models
{
    internal class PlayerPresence
    {

            public bool isValid { get; set; }
            public string sessionLoopState { get; set; }
            public string partyOwnerSessionLoopState { get; set; }
            public string customGameName { get; set; }
            public string customGameTeam { get; set; }
            public string partyOwnerMatchMap { get; set; }
            public string partyOwnerMatchCurrentTeam { get; set; }
            public int partyOwnerMatchScoreAllyTeam { get; set; }
            public int partyOwnerMatchScoreEnemyTeam { get; set; }
            public string partyOwnerProvisioningFlow { get; set; }
            public string provisioningFlow { get; set; }
            public string matchMap { get; set; }
            public string partyId { get; set; }
            public bool isPartyOwner { get; set; }
            public string partyState { get; set; }
            public string partyAccessibility { get; set; }
            public int maxPartySize { get; set; }
            public string queueId { get; set; }
            public bool partyLFM { get; set; }
            public string partyClientVersion { get; set; }
            public int partySize { get; set; }
            public string tournamentId { get; set; }
            public string rosterId { get; set; }
            public long partyVersion { get; set; }
            public string queueEntryTime { get; set; }
            public string playerCardId { get; set; }
            public string playerTitleId { get; set; }
            public string preferredLevelBorderId { get; set; }
            public int accountLevel { get; set; }
            public int competitiveTier { get; set; }
            public int leaderboardPosition { get; set; }
            public bool isIdle { get; set; }
        

    }
}
