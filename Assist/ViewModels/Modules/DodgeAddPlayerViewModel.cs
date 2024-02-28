using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Services.Assist;
using AssistUser.Lib.V2.Models;
using AssistUser.Lib.V2.Models.APDB;
using AssistUser.Lib.V2.Models.Dodge;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.Logging;
using Log = Serilog.Log;

namespace Assist.ViewModels.Modules;

public partial class DodgeAddPlayerViewModel : ViewModelBase
{
    
    [ObservableProperty] private ICommand? _closeViewCommand;
    [ObservableProperty] private string _playerNameText;
    [ObservableProperty] private string _playerNoteText;
    [ObservableProperty] private string _dodgeSelectedTitle;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private int _playerSelectedCategory = -1;
    [ObservableProperty] private bool _isProcessing = false;
    
    
    [RelayCommand]
    public async Task AddPlayer()
    {
        if (PlayerSelectedCategory < 0)
        {
            ErrorMessage = "Please Select a Category.";
            return;
        }

        var splitName = PlayerNameText.Split("#");
        if (splitName.Length != 2 )
        {
            ErrorMessage = "Please correct the format to NAME#TAG";
            return;
        }
        
        IsProcessing = true;
        
        // Make request to validate username to APDB
        APDBPlayer? playerData;
        try
        {
            var request = await AssistApplication.AssistUser.DodgeList.APDBGetPlayerInformation(splitName[0], splitName[1]);

            if (request.Code != 200)
            {
                ErrorMessage = request.Message;
                IsProcessing = false;
                return;
            }

            playerData = JsonSerializer.Deserialize<APDBPlayer>(request.Data.ToString());
        }
        catch (Exception e)
        {
            Log.Error("Failed to get player account while adding on dodge list.");
            ErrorMessage = e.Message;
            IsProcessing = false;
            return;
        }

        if (playerData is null)
        {
            Log.Error("Failed to get player account while adding on dodge list.");
            ErrorMessage = "Failed to get Player Data";
            IsProcessing = false;
            return;
        }

        var data = new AddUserToDodgeListModel()
        {
            Category = (EAssistDodgeCategory)PlayerSelectedCategory,
            Note = PlayerNoteText,
            RiotId = playerData.Id
        };

        try
        {
            var resp = await DodgeService.Current.AddPlayerToUserDodgeList(data);
        }
        catch (Exception e)
        {
            Log.Error("Failed to add player to dodge list.");
            ErrorMessage = e.Message;
            IsProcessing = false;
            return;
        }
    }
}