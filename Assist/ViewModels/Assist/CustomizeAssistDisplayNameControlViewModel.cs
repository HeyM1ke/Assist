using System;
using System.Windows.Input;
using AssistUser.Lib.Base.Exceptions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;


namespace Assist.ViewModels.Assist;

public partial class CustomizeAssistDisplayNameControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _canClose = false;
    [ObservableProperty] private ICommand? _closeCommand = null;
    [ObservableProperty] private ICommand? _finishCommand = null;
    [ObservableProperty] private string _displayNameText;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _errorVisible;
    [ObservableProperty] private bool _isProcessing = false;
    public async void Setup()
    {
        CanClose = CloseCommand is null; // Disables the Close Button if there isn't one.
    }


    [RelayCommand]
    public async void ChangeDisplayName()
    {
        IsProcessing = true;
        ErrorVisible = false;
        if (DisplayNameText.Length < 4 || DisplayNameText.Length > 16)
        {
            ChangeErrorMessage("Name needs to be 4-16 Characters");
            return;
        }


        try
        {
            var resp = await AssistApplication.AssistUser.Account.ChangeDisplayName(DisplayNameText);

            if (resp.Code != 200)
            {
                ChangeErrorMessage(resp.Message);
                IsProcessing = false;
                return;
            }
            
            Log.Information("Changed Displayname.");
        }
        catch (RequestException e)
        {
            ChangeErrorMessage(e.Message);
            Log.Error("Failed to change displayname");
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
            IsProcessing = false;
            return;
        }
        
        FinishCommand?.Execute(null);
    }


    private void ChangeErrorMessage(string message)
    {
        ErrorVisible = true;
        ErrorMessage = message;
    }
}