using Assist.Game.Services;
using Assist.Game.Views.Lobbies.Pages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Game.Views.Lobbies;

public partial class LobbiesView : UserControl
{
    public LobbiesView()
    {
        GameViewNavigationController.CurrentPage = Page.LOBBIES;
        InitializeComponent();
    }



    private void BrowserView_Loaded(object? sender, RoutedEventArgs e)
    {
        Log.Information("Browser View Active");
        var bV = sender as BrowserView;
        if (bV != null)
        {
            bV.ReloadBrowser();
        }
    }
}