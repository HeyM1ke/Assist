using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assist.Controls.Assist;
using Assist.Controls.Navigation;
using Assist.Models.Enums;
using Assist.Services.Modules;
using Assist.Shared.Settings.Modules;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class DiscordRPViewModel : ViewModelBase
{
    /// This is a really Janky Setup btw, but since its toggle buttons this is a bit of a pain.

    [ObservableProperty] private bool _discordEnabled;

    private bool _setup = true;

    [ObservableProperty]private ObservableCollection<WideToggleButton> _largeImageButtons = new ObservableCollection<WideToggleButton>();
    [ObservableProperty]private ObservableCollection<EnableDisableTextButton> _privacyButtons = new ObservableCollection<EnableDisableTextButton>();
    [ObservableProperty]private ObservableCollection<WideToggleButton> _smallImageButtons = new ObservableCollection<WideToggleButton>();

    public async void Setup()
    {
        CreateLargeImageButtons();
        CreateSmallImageButtons();
        CreatePrivacyButtons();
        DiscordEnabled = ModuleSettings.Default.RichPresenceSettings.IsEnabled;
        _setup = false;
    }
    
    
    [RelayCommand]
    public void HandlePrivacyChange(string type)
    {
        switch (type)
        {
            case "score":
                ModuleSettings.Default.RichPresenceSettings.ShowScore = !ModuleSettings.Default.RichPresenceSettings.ShowScore;
                break;
            case "rank":
                ModuleSettings.Default.RichPresenceSettings.ShowRank = !ModuleSettings.Default.RichPresenceSettings.ShowRank;
                break;
            case "mode":
                ModuleSettings.Default.RichPresenceSettings.ShowMode = !ModuleSettings.Default.RichPresenceSettings.ShowMode;
                break;
            case "agent":
                ModuleSettings.Default.RichPresenceSettings.ShowAgent = !ModuleSettings.Default.RichPresenceSettings.ShowAgent;
                break;
            case "party":
                ModuleSettings.Default.RichPresenceSettings.ShowParty = !ModuleSettings.Default.RichPresenceSettings.ShowParty;
                break;
        }
        
        ModuleSettings.Save();
        RichPresenceService.Default.ForceUpdate();
    }

    private void CreateLargeImageButtons()
    {
        LargeImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeLargeImageDataCommand,
            CommandParameter = ERPDataType.MAP,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.LargeImageData == ERPDataType.MAP,
            Content = "Map"
        });
        
        LargeImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeLargeImageDataCommand,
            CommandParameter = ERPDataType.RANK,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.LargeImageData == ERPDataType.RANK,
            Content = "Rank"
        });
        
        LargeImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeLargeImageDataCommand,
            CommandParameter = ERPDataType.AGENT,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.LargeImageData == ERPDataType.AGENT,
            Content = "Agent"
        });
        
        LargeImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeLargeImageDataCommand,
            CommandParameter = ERPDataType.LOGO,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.LargeImageData == ERPDataType.LOGO,
            Content = "Logo"
        });
        
        LargeImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeLargeImageDataCommand,
            CommandParameter = ERPDataType.NONE,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.LargeImageData == ERPDataType.NONE,
            Content = "None"
        });
    }
    
    private void CreateSmallImageButtons()
    {
        SmallImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeSmallImageDataCommand,
            CommandParameter = ERPDataType.MAP,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.SmallImageData == ERPDataType.MAP,
            Content = "Map"
        });
        
        SmallImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeSmallImageDataCommand,
            CommandParameter = ERPDataType.RANK,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.SmallImageData == ERPDataType.RANK,
            Content = "Rank"
        });
        
        SmallImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeSmallImageDataCommand,
            CommandParameter = ERPDataType.AGENT,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.SmallImageData == ERPDataType.AGENT,
            Content = "Agent"
        });
        
        SmallImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeSmallImageDataCommand,
            CommandParameter = ERPDataType.LOGO,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.SmallImageData == ERPDataType.LOGO,
            Content = "Logo"
        });
        
        SmallImageButtons.Add(new WideToggleButton()
        {
            Command = ChangeSmallImageDataCommand,
            CommandParameter = ERPDataType.NONE,
            IsChecked = ModuleSettings.Default.RichPresenceSettings.SmallImageData == ERPDataType.NONE,
            Content = "None"
        });
    }
    
    private void CreatePrivacyButtons()
    {
        PrivacyButtons.Add(new EnableDisableTextButton()
        {
            Command = HandlePrivacyChangeCommand,
            CommandParameter = "party",
            IsChecked = ModuleSettings.Default.RichPresenceSettings.ShowParty,
            Content = "Show Party"
        });
        
        PrivacyButtons.Add(new EnableDisableTextButton()
        {
            Command = HandlePrivacyChangeCommand,
            CommandParameter = "agent",
            IsChecked = ModuleSettings.Default.RichPresenceSettings.ShowAgent,
            Content = "Show Agent"
        });
        
        PrivacyButtons.Add(new EnableDisableTextButton()
        {
            Command = HandlePrivacyChangeCommand,
            CommandParameter = "mode",
            IsChecked = ModuleSettings.Default.RichPresenceSettings.ShowMode,
            Content = "Show Mode"
        });
        
        PrivacyButtons.Add(new EnableDisableTextButton()
        {
            Command = HandlePrivacyChangeCommand,
            CommandParameter = "rank",
            IsChecked = ModuleSettings.Default.RichPresenceSettings.ShowRank,
            Content = "Show Rank"
        });
        
        PrivacyButtons.Add(new EnableDisableTextButton()
        {
            Command = HandlePrivacyChangeCommand,
            CommandParameter = "score",
            IsChecked = ModuleSettings.Default.RichPresenceSettings.ShowScore,
            Content = "Show Score"
        });
       
    }
    
    
    [RelayCommand]
    public void ChangeSmallImageData(ERPDataType type)
    {
        if (ModuleSettings.Default.RichPresenceSettings.SmallImageData != type)
        {
            ModuleSettings.Default.RichPresenceSettings.SmallImageData = type;
            ModuleSettings.Save();
            RichPresenceService.Default.ForceUpdate();
        }
    }
    
    [RelayCommand]
    public void ChangeLargeImageData(ERPDataType type)
    {
        if (ModuleSettings.Default.RichPresenceSettings.LargeImageData != type)
        {
            ModuleSettings.Default.RichPresenceSettings.LargeImageData = type;
            ModuleSettings.Save();
            RichPresenceService.Default.ForceUpdate();
        }
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

    public async void HandleDiscordEnableChange()
    {
        if (_setup) return;
        Log.Information("User has asked to change if discord Pres is enabled");
        DiscordEnabled = !DiscordEnabled;
        if (!DiscordEnabled && ModuleSettings.Default.RichPresenceSettings.IsEnabled)
        {
            // RPc needs to turn off. 
            if (RichPresenceService.Default.BDiscordPresenceActive)
            {
                await RichPresenceService.Default.Shutdown();
            }
        }
        
        if (DiscordEnabled)
        {
            if (!RichPresenceService.Default.BDiscordPresenceActive) {
                RichPresenceService.Default.Initialize();
            }
        }
        
        ModuleSettings.Default.RichPresenceSettings.IsEnabled = DiscordEnabled;

        ModuleSettings.Save();
    }
}