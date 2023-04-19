using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Services.Leagues;
using Assist.ViewModels;
using AssistUser.Lib.Parties.Models;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeagueQueueViewModel : ViewModelBase
{

    private string _buttonText;

    public string ButtonText
    {
        get => _buttonText;
        set => this.RaiseAndSetIfChanged(ref _buttonText, value);
    }
    
    private bool _buttonEnabled;

    public bool ButtonEnabled
    {
        get => _buttonEnabled;
        set => this.RaiseAndSetIfChanged(ref _buttonEnabled, value);
    }

    private AssistPartyMember _partyMember;
    
    public async Task QueueIntoLeague()
    {
        var resp = await AssistApplication.Current.AssistUser.League.JoinQueue(LeagueService.Instance.CurrentLeagueId);
        Log.Information($"CODE {resp.Code}");
        Log.Information($"MESSAGE {resp!.Message}");
    }
    
    public async Task ButtonClick()
    {
        if (_partyMember.IsLeader)
        {
            var resp = await AssistApplication.Current.AssistUser.League.JoinQueue(LeagueService.Instance.CurrentLeagueId);
            Log.Information($"CODE {resp.Code}");
            Log.Information($"MESSAGE {resp!.Message}");
            return;
        }

        if (!_partyMember.IsReady)
        {
            Log.Information($"Attempting to Ready from party.");
            var resp = await AssistApplication.Current.AssistUser.Party.SetReadyStatusInParty(LeagueService.Instance.CurrentPartyInfo.Id, new ReadyStatus()
            {
                IsReady = true
            });
            Log.Information($"CODE {resp.Code}");
            Log.Information($"MESSAGE {resp!.Message}");
        }
        else
        {
            Log.Information($"Attempting to Unready from party.");
            var resp = await AssistApplication.Current.AssistUser.Party.SetReadyStatusInParty(LeagueService.Instance.CurrentPartyInfo.Id, new ReadyStatus()
            {
                IsReady = false
            });
            Log.Information($"CODE {resp.Code}");
            Log.Information($"MESSAGE {resp!.Message}");
        }
        
    }

    public async Task Setup()
    {
        // Check if the player is a member or leader
        AssistApplication.Current.GameServerConnection.PARTY_PartyUpdateReceived += GameServerConnectionOnPARTY_PartyUpdateReceived;
        await LeagueService.Instance.GetCurrentPartyData();
        await SearchPartyDetails(LeagueService.Instance.CurrentPartyInfo);
    }

    private async void GameServerConnectionOnPARTY_PartyUpdateReceived(string? obj)
    {
        Log.Information("Queue Button has received an Update.");
        var pty = JsonSerializer.Deserialize<AssistParty>(obj,new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        await SearchPartyDetails(pty);
    }


    public async Task SearchPartyDetails(AssistParty pty)
    {
        var playerData = pty.Members.Find(mem => mem.Id == AssistApplication.Current.AssistUser.Account.AccountInfo.id);
        if (playerData is null)
        {
            Log.Error("This is a fuckign issue");
            Log.Error("Player is not found within the party details that was recieved");
            return;
        }

        playerData.IsLeader = pty.LeagueId == playerData.Id;
        _partyMember = playerData;

        if (playerData.IsLeader)
        {
            var notreadyplayer = pty.Members.Exists(mem => !mem.IsReady && mem.IsLeader != _partyMember.IsLeader);
            if (notreadyplayer)
            {
                ButtonText = "Waiting for Players to ready.";
                ButtonEnabled = false;
                return;
            }
            ButtonText = "QUEUE";
            ButtonEnabled = true;
        }
        else
        {
            if (_partyMember.IsReady)
            {
                ButtonText = "Waiting...";
                ButtonEnabled = true;
                return;
            }
            else
            {
                ButtonText = "READY";
                ButtonEnabled = true;
                return;
            }
        }
    }
}