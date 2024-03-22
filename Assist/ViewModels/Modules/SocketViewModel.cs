using System;
using Assist.Controls.Navigation;
using Assist.Services.Modules;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class SocketViewModel : ViewModelBase
{

    [ObservableProperty] private string _socketAddressText;
    [ObservableProperty] private string _socketButtonText = Properties.Resources.Common_Connect;
    [ObservableProperty] private bool _socketTextEnabled = true;
    [ObservableProperty] private bool _isLoading = false;
    
    [ObservableProperty] private bool _allowReconnectIfFail = false;
    
    
    [ObservableProperty] private string _errorMessageText = Properties.Resources.Common_Connect;
    [ObservableProperty] private bool _errorMessageEnabled = false;
    
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
    public void ConnectToSocket()
    {
        ErrorMessageEnabled = false;
        Log.Information("Player is attempting start/stop the socket.");

        if (string.IsNullOrEmpty(SocketAddressText))
        {
            ChangeErrorMessage("Socket address cannot be empty.");
            return;
        }

        SocketService.Instance.OnDisconnect += () =>
        {
            SocketTextEnabled = true;
            SocketButtonText = Properties.Resources.Common_Connect;

            if (AllowReconnectIfFail)
            {
                try
                {
                    SocketService.Instance.Connect(SocketAddressText);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to reconnect on Allowing to reconnect.");
                }
            }
        };

        SocketService.Instance.OnConnect += () =>
        {
            SocketTextEnabled = false;
            SocketButtonText = Properties.Resources.Common_Disconnect;
        };
        
        if (SocketService.Instance.IsConnected)
        {
            SocketService.Instance.Disconnect();
            SocketTextEnabled = true;
            SocketButtonText = Properties.Resources.Common_Connect;
        }
        else
        {
            try
            {
                SocketService.Instance.Connect(SocketAddressText);
            }
            catch (Exception e)
            {
                ChangeErrorMessage(e.Message);
                return;
            }

        }
    }


    [RelayCommand]
    public void CreateNewSession()
    {
        Log.Information("Player is attempting create a new session on the socket.");
        if (SocketService.Instance.IsConnected)
            SocketService.Instance.CreateNewSession();
    }


    private void ChangeErrorMessage(string message)
    {
        ErrorMessageText = message;
        ErrorMessageEnabled = true;
    }
}