using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeaguePartyMemberViewModel : ViewModelBase
{

    private string _playerName = "Player";
    public string PlayerName
    {
        get => _playerName;
        set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }
    
    private string _playerRanking = "Rank: #0 // LP: 0";
    public string PlayerRanking
    {
        get => _playerRanking;
        set => this.RaiseAndSetIfChanged(ref _playerRanking, value);
    }

    public async void UpdatePartyMember(object data)
    {
        
    }
    
    
    private void SetupLeagueText()
    {
        
    }
    
}