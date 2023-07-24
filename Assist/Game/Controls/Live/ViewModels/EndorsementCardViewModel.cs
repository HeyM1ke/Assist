using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Reputations.Models;
using ReactiveUI;
using ValNet.Objects.Player;

namespace Assist.Game.Controls.Live.ViewModels;

public class EndorsementCardViewModel : ViewModelBase
{
    public MatchDetailsObj.Player PlayerData { get; set; }
    public string MatchId { get; set; }

    private bool _playerEndorsementsEnabled = true;
    public bool PlayerEndorsementsEnabled
    {
        get => _playerEndorsementsEnabled;
        set => this.RaiseAndSetIfChanged(ref _playerEndorsementsEnabled, value);
    }
    
    private string? _playerName = "Player";

    public string? PlayerName
    {
        get => _playerName;
        set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }
    
    private string? _playerCard = "Player";

    public string? PlayerCard
    {
        get => _playerCard;
        set => this.RaiseAndSetIfChanged(ref _playerCard, value);
    }
    
    private string? _agentImage = "Player";

    public string? AgentImage
    {
        get => _agentImage;
        set => this.RaiseAndSetIfChanged(ref _agentImage, value);
    }
    private string? _playerStats = "0 / 0 / 0";

    public string? PlayerStats
    {
        get => _playerStats;
        set => this.RaiseAndSetIfChanged(ref _playerStats, value);
    }
    public async Task Setup()
    {
        if (PlayerData is null)
        {
            return;
        }

        PlayerCard = $"https://content.assistapp.dev/playercards/{PlayerData.PlayerCard}_LargeArt.png";
        PlayerName = PlayerData.GameName;
        PlayerStats = $"{PlayerData.Stats.Kills} / {PlayerData.Stats.Deaths} / {PlayerData.Stats.Assists}";
        AgentImage = $"https://content.assistapp.dev/agents/{PlayerData.CharacterId}_fullportrait.png";
    }

    public async Task EndorsePlayer(EndorsementTypeV2 type)
    {
        PlayerEndorsementsEnabled = false;
        var t = await AssistApplication.Current.AssistUser.Reputation.EndorseUserV2(PlayerData.Subject, type, MatchId);
        if (t.Code != 200)
        {
            await AssistApplication.Current.ShowNotification(Properties.Resources.Global_Notification, HandleErrorMessage(t.Message));
            PlayerEndorsementsEnabled = true;
            return;
        }
        
        await AssistApplication.Current.ShowNotification("Notification", "Successfully Endorsed Player");
    }

    private string HandleErrorMessage(string message)
    {
        switch (message)
        {
            case "MAXENDORSEPERMATCH":
                return "Maximum Endorsements for this match has been reached.";
            case "RIOTACCNOTLINKED":
                return "Riot account needs to be linked to access this feature.";
            case "ENDORSE24HOURLIMIT":
                return "Cannot endorse the same player within 24 hours.";
            case "ENDORSEREPEATINGPLAYER":
                return "Cannot Endorse the same player twice.";
            default:
                return message;
        }
    }
}