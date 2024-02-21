using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Game.Controls.Live;
using Assist.Game.Services;
using Assist.ViewModels;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Live.Pages.ViewModels;

public class EndorsePageViewModel : ViewModelBase
{
    private ObservableCollection<EndorsementCard> _endorsementCards = new ObservableCollection<EndorsementCard>();

    public ObservableCollection<EndorsementCard> EndorsementCards
    {
        get => this._endorsementCards;
        set => this.RaiseAndSetIfChanged(ref _endorsementCards, value);
    }

    public async Task Setup()
    {
        var matchHistory = await AssistApplication.Current.CurrentUser.Player.GetPlayerMatchHistory(0, 3);

        if (matchHistory.History.Count <= 0)
        {
            Log.Information("Player has no Match History");
            GameViewNavigationController.Change(new LiveView());
            return;
        }

        if (string.IsNullOrEmpty(matchHistory.History[0].QueueID) || matchHistory.History[0].QueueID.Equals("deathmatch", StringComparison.OrdinalIgnoreCase ))
        {
            Log.Information("Match Is not supported gamemode");
            GameViewNavigationController.Change(new LiveView());
            return;
        }
        
        var matchDetails = await AssistApplication.Current.CurrentUser.Player.GetMatchDetails(matchHistory.History[0].MatchID);

        var localPlayer = matchDetails.Players.Find(x => x.Subject == AssistApplication.Current.CurrentUser.UserData.sub);

        foreach (var teammate in matchDetails.Players.FindAll(x=> x.TeamId == localPlayer.TeamId))
        {
            if (teammate.Subject == localPlayer.Subject)
            {
                continue;
            }

            Log.Information("adding new card");
            var card = new EndorsementCard(teammate, matchHistory.History[0].MatchID);
            EndorsementCards.Add(card);
        }
    }
}