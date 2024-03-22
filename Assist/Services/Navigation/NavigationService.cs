using Assist.Controls.Navigation;
using Assist.Core.Helpers;
using Assist.ViewModels;
using Assist.ViewModels.Navigation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Assist.Services.Navigation;

public static class NavigationService
{
    public static AssistPage CurrentPage { get; set; } = AssistPage.UNKNOWN;
    public static NavigationViewModel CurrentViewModel;

    public static void SetViewModel(NavigationViewModel _vm)
    {
        CurrentViewModel = _vm;
    }

    public static void AddButton(NavigationButton navBtn)
    {
        CurrentViewModel.NavigationButtons.Add(navBtn);
    }
    
    public static void RemoveButton(AssistPage page)
    {
        var btn = CurrentViewModel.NavigationButtons.Find(btn => btn.Page == page );
        if (btn is not null)
            CurrentViewModel.NavigationButtons.Remove(btn);
    }
    
    public static bool EnableButton(AssistPage page)
    {
        var btn = CurrentViewModel.NavigationButtons.Find(btn => btn.Page == page );
        if (btn is not null)
            btn.IsVisible = true;
        else
            return false;

        return btn.IsVisible;
    }
    
    public static bool DisableButton(AssistPage page)
    {
        var btn = CurrentViewModel.NavigationButtons.Find(btn => btn.Page == page );
        if (btn is not null)
            btn.IsVisible = false;
        else
            return false;

        return btn.IsVisible;
    }

    public static NavigationButton CreateButton(AssistPage Page, UserControl PageControl, string SvgImagePath,
        bool Selected = false)
    {
        var btn = new NavigationButton()
        {
            Icon = ImageHelper.LoadFromResourceSVG(SvgImagePath),
            IsChecked = Selected,
            Page = Page
        };

        //TODO: Add Click Action/Command
        /*btn.Click += delegate(object? sender, RoutedEventArgs args)
        {
            var t = sender as NavigationButton;
            if (NavigationService.CurrentPage != t.Page)
                AssistApplication.ChangeMainWindowView(PageControl);
        };*/
        
        return btn;
    }
    
    
}

public enum AssistPage
{
    UNKNOWN,
    STARTUP,
    DASHBOARD,
    STORE,
    INVENTORY,
    PROFILE,
    MODULES,
    SETTINGS,
    LIVE,
    RACCOUNT,
    SETUP,
    SWAP,
    LEAGUE,
    DODGE,
    DISCORD,
    ASSSOCKET,
    EXTENSION,
}