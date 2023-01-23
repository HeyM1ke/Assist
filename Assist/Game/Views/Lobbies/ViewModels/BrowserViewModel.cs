using System.Collections.Generic;
using System.Threading.Tasks;
using Assist.Game.Controls.Lobbies;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Game.Views.Lobbies.ViewModels;

public class BrowserViewModel : ViewModelBase
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private List<object> AllLobbiesStorage;

    private List<LobbyBrowserPreviewControl> _currentlyShownLobbies;

    public List<LobbyBrowserPreviewControl> CurrentlyShownLobbies
    {
        get => _currentlyShownLobbies;
        set => this.RaiseAndSetIfChanged(ref _currentlyShownLobbies, value);
    }
    
    public async Task Setup()
    {
        // Set Loading to True.
        
        // Get Users Region
        // Make Request to get Lobbies
        // Store all Lobbies in Storage.
        
        // Create Controls and Set them to the Shown list. 
    }

    public async Task FilterList()
    {
        // Search Storage List Names.
        // Replace List for any matching names or semi matching.
        
    }
}