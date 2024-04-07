using System.Timers;
using Assist.Controls.Infobars;
using Assist.Services.Riot;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.Views.Extras;

public partial class VLRLaunchedPage : UserControl
{
    private readonly VLRLaunchPageViewModel _viewModel;

    public VLRLaunchedPage()
    {
        DataContext = _viewModel = new VLRLaunchPageViewModel();
        InitializeComponent();
    }

    private void VLRLaunchPage_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        Titlebar.ViewModel.SettingsEnabled = false;
        Titlebar.ViewModel.AccountSwapEnabled = false;
        
        _viewModel.SetTimer();
    }

    private void VLRLaunchPage_Unloaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.StopTimer();
    }
}

internal partial class VLRLaunchPageViewModel : ViewModelBase
{
    [ObservableProperty]private Timer _checkingTimer;
    private int _timerCheckInSeconds = 20;

    [ObservableProperty] private bool _loadingLongButtonVisible = false;
    
    public void SetTimer()
    {
        _checkingTimer = new Timer(_timerCheckInSeconds * 1000);
        _checkingTimer.Elapsed += TimerOnComplete;
        _checkingTimer.AutoReset = false;
        _checkingTimer.Enabled = true;
    }

    private void TimerOnComplete(object? sender, ElapsedEventArgs e)
    {
        Log.Information("Valorant Loading Timer has Completed");

        LoadingLongButtonVisible = true;
    }

    public void StopTimer()
    {
        _checkingTimer.Stop();
        _checkingTimer.Dispose();
    }

    [RelayCommand]
    public async void HandleLongTimeCommand()
    {
        if (RiotClientService.Instance is null)
        {
            Log.Error("How the hell did you even get to this page????");
            return;
        }
        
        Log.Information("Stopping Background Launch Worker");
        RiotClientService.Instance.StopWorker();
        await RiotClientService.CloseRiotRelatedPrograms();
        Log.Information("Repairing Profile, Deleting ZIP Backup.");
        new RiotClientService().RemoveAnyExistingClientDataFiles();

        await RiotClientService.Instance.StartClient();
        // Restart Timer JUST IN CASE
        SetTimer();
    }
} 