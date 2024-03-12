using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Assist.Services.Assist;
using AssistUser.Lib.V2.Models;
using AssistUser.Lib.V2.Models.APDB;
using AssistUser.Lib.V2.Models.Dodge;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class DodgeQuickAddPlayerViewModel : ViewModelBase{
    [ObservableProperty] private ICommand? _closeViewCommand;
    [ObservableProperty] private string _playerId;
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
        
        IsProcessing = true;
        
        var data = new AddUserToDodgeListModel()
        {
            Category = (EAssistDodgeCategory)PlayerSelectedCategory,
            Note = PlayerNoteText,
            RiotId = PlayerId
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
        
        AssistApplication.ChangeMainWindowPopupView(null);
    }


    [RelayCommand]
    public void ClosePopup()
    {
        AssistApplication.ChangeMainWindowPopupView(null);
    }
}