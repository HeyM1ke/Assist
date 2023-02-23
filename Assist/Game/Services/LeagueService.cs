using System.Collections.Generic;

namespace Assist.Game.Services;

// Class that handles League Functions
public class LeagueService
{
    public static LeagueService Current;
    /// <summary>
    /// Current Selected League Id
    /// </summary>
    private string _currentLeagueId;
    
    /// <summary>
    /// List of Leagues Player Belongs too.
    /// </summary>
    private List<object> _entitledLeagues;

    public LeagueService()
    {
        Current = this;
    }
    
    
}