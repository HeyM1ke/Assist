using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Services.Leagues;
using Assist.Game.Views.Leagues.Popup;
using Assist.Services.Popup;
using Assist.ViewModels;
using AssistUser.Lib.Parties.Models;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeaguePartyControlViewModel : ViewModelBase
{
    // is Loading
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    
    private string _partyMemberCount = "1/5";

    public string PartyMemberCount
    {
        get => _partyMemberCount;
        set => this.RaiseAndSetIfChanged(ref _partyMemberCount, value);
    }
    // List for all current objects
    private ObservableCollection<Control> _partyControls = new ObservableCollection<Control>();
    
    public ObservableCollection<Control> PartyControls
    {
        get => _partyControls;
        set => this.RaiseAndSetIfChanged(ref _partyControls, value);
    }
    
    

    // Update Method
    
    // Add Method
    // Remove Method
    // Replace Method
    // Bind to Events

    public async Task BindToEvents()
    {
        AssistApplication.Current.GameServerConnection.PARTY_PartyUpdateReceived += GameServerConnectionOnPARTY_PartyUpdateReceived;
        AssistApplication.Current.GameServerConnection.PARTY_PartyKickReceived += GameServerConnectionOnPARTY_PartyKickReceived;
    }
    
    public async Task UnbindToEvents()
    {
        AssistApplication.Current.GameServerConnection.PARTY_PartyUpdateReceived -= GameServerConnectionOnPARTY_PartyUpdateReceived;
        AssistApplication.Current.GameServerConnection.PARTY_PartyKickReceived -= GameServerConnectionOnPARTY_PartyKickReceived;
    }

    private void GameServerConnectionOnPARTY_PartyKickReceived(string? obj)
    {
        
    }

    private async void GameServerConnectionOnPARTY_PartyUpdateReceived(string? obj)
    {
        Log.Information("RECEIVED PARTY UPDATE MESSAGE ON CONTROLVIEWMODEL FOR PARTY CONTROL");
        var pty = JsonSerializer.Deserialize<AssistParty>(obj);

        LeagueService.Instance.CurrentPartyInfo = pty;
        UpdateObjects(pty);
    }

    // Create new Party if League ID != same.
    // 
    public async Task Initialize()
    {
        IsLoading = true;
        // Check if there is a current party
        var pty = await LeagueService.Instance.GetCurrentPartyData();

        if (pty.LeagueId != LeagueService.Instance.CurrentLeagueId)
        {
            Log.Information("Current Party does not equal the current league, creating new party");
            pty = await LeagueService.Instance.CreateNewParty();
        }
        PartyMemberCount = $"{pty.CurrentSize}/{pty.MaxSize}";
        // Get all members and generate their objects
        await UpdateObjects(pty);

        IsLoading = false;
    }

    private async Task UpdateObjects(AssistParty partyData)
    {
        Log.Information("Updating Party Member Objects");
        if (partyData.Members.Count <= 0)
        {
            Log.Information("We have an issue, Party has zero members");
        }
        
        
        for (int i = 0; i < partyData.CurrentSize; i++)
        {
            var control = new LeaguePartyMemberControl(partyData.Members[i]);
            AddToControls(control);
        }
        
        for (int i = 0; i < (partyData.MaxSize - partyData.CurrentSize); i++)
        {
            var control = new LeaguePartyInviteControl();
            control.Click += InviteControl_Click;
            AddToControls(control);
        }
    }

    private void InviteControl_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.SpawnCustomPopup(new InvitePlayerPartyView());
    }


    private async Task AddToControls(Control control)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            PartyControls.Add(control);
        });
    }
    
    private async Task RemoveFromControls(Control control)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            PartyControls.Remove(control);
        });
    }
    
    private async Task ClearControls(Control control)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            PartyControls.Clear();
        });
    }
    
    private async Task ReplaceFromControl(int indexToReplace, Control control)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            PartyControls.RemoveAt(indexToReplace);
            PartyControls.Insert(indexToReplace,control);
            
        });
    }
}