using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Modules.Dodge;
using Assist.Controls.Navigation;
using Assist.Core.Helpers;
using Assist.Services.Assist;
using Assist.Services.Navigation;
using AssistUser.Lib.V2.Models.Dodge;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using ValNet.Core.Player;

namespace Assist.ViewModels.Modules;

public partial class DodgeViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<DodgePlayerPreviewControl> _playerControls = new ObservableCollection<DodgePlayerPreviewControl>();
    [ObservableProperty] private Control _popupControl;
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _isListEmpty = false;

    /// <summary>
    /// Method gets called when the page is loaded.
    /// </summary>
    public async Task Load()
    {
        IsLoading = true;
        if (DodgeService.Current is null)
        {
            Log.Information("DodgeService is Null, Starting up a service instance.");
            new DodgeService();
        }

        if (string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken))// Simple Check before real checks later.
        {
            Log.Information("User somehow loaded Assist Dodge View, without a token.");
            Dispatcher.UIThread.Invoke(() =>
            {
                NavigationContainer.ViewModel.ChangePage(AssistPage.MODULES);
            });
            return;   
        }
        CreateDodgeControls();
        
        Log.Information("Dodge View Loaded, Subscribing");
        DodgeService.Current.DodgeUserAddedToList += DodgeUserAddedToList;
        DodgeService.Current.DodgeUserRemovedFromList += DodgeUserRemovedFromList;
        IsLoading = false;
    }

    public void Unload()
    {
        Log.Information("Dodge View Unloaded, Unsubscribing");
        PlayerControls.Clear();
        DodgeService.Current.DodgeUserAddedToList -= DodgeUserAddedToList;
        DodgeService.Current.DodgeUserRemovedFromList -= DodgeUserRemovedFromList;
    }
    
    private async void CreateDodgeControls()
    {
        await DodgeService.Current.UpdateDodgeList();
        for (int i = 0; i < DodgeService.Current.DodgeList.Players.Count; i++)
        {
            var p = DodgeService.Current.DodgeList.Players[i];
            
            PlayerControls.Add(new DodgePlayerPreviewControl()
            {
                PlayerId = p.PlayerId,
                PlayerName = p.AddedAs is null ? "Player" : $"{p.AddedAs.GameName}#{p.AddedAs.TagLine}",
                PlayerCategory = AssistHelper.DodgeCategories.ContainsKey((EAssistDodgeCategory)p.Category) ? $"{AssistHelper.DodgeCategories[(EAssistDodgeCategory)p.Category]}" : "Not Found",
                PlayerNote = p.Note,
                NoteEnabled = !string.IsNullOrEmpty(p.Note),
                DateAdded = $"{p.Added.ToLocalTime().ToShortDateString()}",
                EditPlayerCommand = OpenPlayerEditPopupCommand,
                DeletePlayerCommand = DeletePlayerFromListCommand
            });  
        }
        
        IsListEmpty = DodgeService.Current.DodgeList.Players.Count == 0;
    }
    
    private void DodgeUserRemovedFromList(UserDodgePlayer? obj)
    {
        Log.Information("Player has been removed from the list.");

        Dispatcher.UIThread.Invoke(() => { 
            var onList = PlayerControls.FirstOrDefault(x => x.PlayerId == obj.PlayerId);
            if (onList is null) return;
            PlayerControls.Remove(onList);
            IsListEmpty = PlayerControls.Count == 0;
        });
        IsListEmpty = PlayerControls.Count == 0;
    }

    private void DodgeUserAddedToList(UserDodgePlayer obj)
    {
        Log.Information("Player has been added to the list.");
        Dispatcher.UIThread.Invoke(() =>
        {
            PlayerControls.Add(new DodgePlayerPreviewControl()
            {
                PlayerId = obj.PlayerId,
                PlayerName = obj.AddedAs is null ? "Player" : $"{obj.AddedAs.GameName}#{obj.AddedAs.TagLine}",
                PlayerCategory = AssistHelper.DodgeCategories.ContainsKey((EAssistDodgeCategory)obj.Category)
                    ? $"{AssistHelper.DodgeCategories[(EAssistDodgeCategory)obj.Category]}"
                    : "Not Found",
                PlayerNote = obj.Note,
                NoteEnabled = !string.IsNullOrEmpty(obj.Note),
                DateAdded = $"{obj.Added.ToLocalTime().ToShortDateString()}",
                EditPlayerCommand = OpenPlayerEditPopupCommand,
                DeletePlayerCommand = DeletePlayerFromListCommand
            });
        });
        
        IsListEmpty = DodgeService.Current.DodgeList.Players.Count == 0;
    }

    [RelayCommand]
    public void ReturnToModules()
    {
        Log.Information("Player is attempting to return to modules page.");
        Dispatcher.UIThread.Invoke(() =>
        {
            NavigationContainer.ViewModel.ChangeToPreviousPage();
        });
    }

    [RelayCommand]
    public void RefreshList()
    {
        Log.Information("Refreshing Dodge List");
        IsLoading = true;
        PlayerControls.Clear();
        CreateDodgeControls();
        IsLoading = false;
    }

    [RelayCommand]
    public void OpenPlayerAddPopup()
    {
        PopupControl = new DodgeAddPlayerControl(ClosePopupsCommand);
    }

    [RelayCommand]
    public void OpenPlayerEditPopup()
    {
        
    }

    [RelayCommand]
    public async void ClearPlayerList()
    {
        try
        {
            IsLoading = true;
            var r = await AssistApplication.AssistUser.DodgeList.ClearPlayerDodgeList();

            if (r.Code != 200)
            {
                Log.Error("Failed to clear Dodgelist");
                Log.Error("CODE: " + r.Code);
                Log.Error("MESSAGE: " + r.Message);
                return;
            }
            
            PlayerControls.Clear();
            IsLoading = false;
        }
        catch (Exception e)
        {
            Log.Error("Failed to clear Dodgelist");
            Log.Error("MESSAGE: " + e.Message);
        }
    }
    
    [RelayCommand]
    public async void DeletePlayerFromList(string id)
    {
        Log.Information("Player Requested to Delete player");
        IsLoading = true;
        try
        {
            var resp = await DodgeService.Current.RemovePlayerFromUserDodgeList(id);
        }
        catch (Exception e)
        {
            IsLoading = false;
            return;
        }
        IsLoading = false;
    }
    
    [RelayCommand]
    public void ClosePopups()
    {
        PopupControl = null;
    }
}