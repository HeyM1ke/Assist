using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assist.Controls.Navigation;
using Assist.Core.Helpers;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.Views.Dashboard;
using Assist.Views.Game.Live;
using Assist.Views.Modules;
using Assist.Views.ProfileSwap;
using Assist.Views.Settings;
using Assist.Views.Store;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Navigation;

public partial class NavigationViewModel : ViewModelBase
{
    public static AssistPage CurrentPage { get; set; } = AssistPage.UNKNOWN;
    
    [ObservableProperty] 
    private List<NavigationButton> _navigationButtons = new ();

    private Dictionary<AssistPage, UserControl> _pages = new Dictionary<AssistPage, UserControl>();
    // Shit you not for some fucking reason this is the only way for this to work right now.
    private AssistPage[] _history = new[] { AssistPage.UNKNOWN, AssistPage.UNKNOWN };

    public NavigationViewModel()
    {
        // Add Buttons to List Programmatically
        CreateControls();
    }

    private void CreateControls()
    {
        _navigationButtons.Add(new NavigationButton()
        {
            Page = AssistPage.DASHBOARD,
            IsVisible = false,
            Command = ChangePageCommand,
            CommandParameter = AssistPage.DASHBOARD,
            Icon = ImageHelper.LoadFromResourceSVG("/Assets/Navigation/Dashboard_Icon.svg")
        });
        
        _navigationButtons.Add(new NavigationButton()
        {
            Page = AssistPage.STORE,
            IsVisible = false,
            Command = ChangePageCommand,
            CommandParameter = AssistPage.STORE,
            Icon = ImageHelper.LoadFromResourceSVG("/Assets/Navigation/Store_Icon.svg")
        });
        
        _navigationButtons.Add(new NavigationButton()
        {
            Page = AssistPage.LIVE,
            IsVisible = false,
            Command = ChangePageCommand,
            CommandParameter = AssistPage.LIVE,
            Icon = ImageHelper.LoadFromResourceSVG("/Assets/Navigation/Inventory_Icon.svg")
        });
        
        _navigationButtons.Add(new NavigationButton()
        {
            Page = AssistPage.MODULES,
            IsVisible = false,
            Command = ChangePageCommand,
            CommandParameter = AssistPage.MODULES,
            Icon = ImageHelper.LoadFromResourceSVG("/Assets/Navigation/Modules_Icon.svg")
        });
        
        _navigationButtons.Add(new NavigationButton()
        {
            Page = AssistPage.INVENTORY,
            IsVisible = false,
            Icon = ImageHelper.LoadFromResourceSVG("/Assets/Navigation/Inventory_Icon.svg")
        });
    }

    public void EnableButton(AssistPage page)
    {
        var btn = _navigationButtons.Find(x => x.Page == page);
        if(btn is null) return;
        btn.IsVisible = true;
    }
    
    public void DisableButton(AssistPage page)
    {
        var btn = _navigationButtons.Find(x => x.Page == page);
        if(btn is null) return;
        btn.IsVisible = false;
    }
    
    
    public void HideAllButtons()
    {
        foreach (var btn in NavigationButtons)
        {
            btn.IsVisible = false;
        }
    }
    
    
    public void EnableAllButtons()
    {
        foreach (var btn in NavigationButtons)
        {
            btn.IsEnabled = true;
        }
    }
    public void DisableAllButtons()
    {
        foreach (var btn in NavigationButtons)
        {
            btn.IsEnabled = false;
        }
    }
    
    public static void SetActivePage(AssistPage _page)
    {
       
        
        CurrentPage = _page;
        
        if (_page == AssistPage.UNKNOWN)
            return;
        
        Dispatcher.UIThread.Invoke(() =>
        {
            var btn = NavigationContainer.ViewModel.NavigationButtons.Find(x => x.Page == _page);
            if(btn is null) return;
            NavigationContainer.ViewModel.NavigationButtons.Select(x => x.IsChecked = false);
            btn.IsChecked = true;
        });
    }

    [RelayCommand]
    public void ChangePage(object t)
    {
        var page = (AssistPage)t;

        if (CurrentPage == page)
            return;

        SetActivePage(page);

        SwapToPage();
    }
    
    
    public void ChangePage(AssistPage page)
    {
        if (CurrentPage == page)
            return;

        SetActivePage(page);

        SwapToPage();
    }
    
    
    public void ChangeToPreviousPage()
    {
        if (_history[0] == AssistPage.UNKNOWN) return;
        var page = _history[0];

        if (CurrentPage == page)
            return;

        SetActivePage(page);

        SwapToPage();
    }
    
    private void SwapToPage()
    {
        if (CurrentPage == AssistPage.UNKNOWN)
            return;
        
        _pages.TryGetValue(CurrentPage, out var newPage);
        
        if (newPage is null)
        {
            switch (CurrentPage)
            {
                case AssistPage.STORE:
                    newPage = new StoreView();
                    break;
                case AssistPage.DASHBOARD:
                    newPage = new DashboardView();
                    break;
                case AssistPage.LIVE:
                    newPage = new LiveView();
                    break;
                case AssistPage.SETTINGS:
                    newPage = new SettingsView();
                    break;
                case AssistPage.MODULES:
                    newPage = new ModulesView();
                    break;
                default:
                    Log.Error("Tried swapping to a page which is not supported. EP01");
                    return;
            }

            _pages.Add(CurrentPage, newPage);
        }

        _history[0] = _history[1];
        _history[1] = CurrentPage;
        
        
        
        AssistApplication.ChangeMainWindowView(newPage);
        GC.Collect();
    }
    
    

    
}
