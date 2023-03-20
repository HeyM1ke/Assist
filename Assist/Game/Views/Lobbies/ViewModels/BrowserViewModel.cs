using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Assist.Game.Controls.Lobbies;
using Assist.Objects.AssistApi.Game;
using Assist.ViewModels;
using AssistUser.Lib.Lobbies.Models;
using ReactiveUI;
using Serilog;
using ValNet.Enums;

namespace Assist.Game.Views.Lobbies.ViewModels;

public class BrowserViewModel : ViewModelBase
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private List<AssistLobby> AllLobbiesStorage;

    private List<LobbyBrowserPreviewControl> _currentlyShownLobbies;

    public List<LobbyBrowserPreviewControl> CurrentlyShownLobbies
    {
        get => _currentlyShownLobbies;
        set => this.RaiseAndSetIfChanged(ref _currentlyShownLobbies, value);
    }
    
    public async Task Setup()
    {
        RefreshList();
    }

    public async Task FilterList(string filter)
    {
        IsLoading = true;
        // Search Storage List Names.
        // Replace List for any matching names or semi matching.
        var filtered = AllLobbiesStorage.Where(lobby => lobby.LobbyName.ToLower().Contains(filter.ToLower())).ToList();
        List<LobbyBrowserPreviewControl> controls = new List<LobbyBrowserPreviewControl>();
        for (int i = 0; i < filtered.Count; i++)
        {
            controls.Add(new LobbyBrowserPreviewControl()
            {
                LobbyName = filtered[i].LobbyName,
                LobbyCode = filtered[i].Code,
                CurrentSize = filtered[i].CurrentPartySize.ToString(),
                MaxSize = filtered[i].MaxPartySize.ToString(),
                IsPasswordProtected = filtered[i].RequiresPassword
            });
        }
        CurrentlyShownLobbies = controls;
        IsLoading = false;
    }
    
    public async Task RefreshList()
    {
        try
        {
            IsLoading = true;
        
            AllLobbiesStorage = await AssistApplication.Current.AssistUser.Lobbies.GetAllLobbies(Enum.GetName(typeof(RiotRegion), AssistApplication.Current.CurrentUser.GetRegion()));

            List<LobbyBrowserPreviewControl> controls = new List<LobbyBrowserPreviewControl>();
            for (int i = 0; i < AllLobbiesStorage.Count; i++)
            {
                controls.Add(new LobbyBrowserPreviewControl()
                {
                    LobbyName = AllLobbiesStorage[i].LobbyName,
                    LobbyCode = AllLobbiesStorage[i].Code,
                    CurrentSize = AllLobbiesStorage[i].CurrentPartySize.ToString(),
                    MaxSize = AllLobbiesStorage[i].MaxPartySize.ToString(),
                    IsPasswordProtected = AllLobbiesStorage[i].RequiresPassword
                });
            }
        
            CurrentlyShownLobbies = controls;
            IsLoading = false;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }
}