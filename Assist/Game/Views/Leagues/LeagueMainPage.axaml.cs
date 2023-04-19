using System;
using System.Threading.Tasks;
using Assist.Game.Controls.Leagues;
using Assist.Game.Services;
using Assist.Game.Services.Leagues;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;
using LeagueService = Assist.Game.Services.Leagues.LeagueService;

namespace Assist.Game.Views.Leagues;

public partial class LeagueMainPage : UserControl
{
    public LeagueMainPage()
    {
        GameViewNavigationController.CurrentPage = Page.LEAGUES;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LeagueMain_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        
        Log.Information("League Page initiated");
        Log.Information("Checking if Leagues have been opened Previously.");

        LeagueNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("LeagueContentControl");
        
        if (LeagueService.Instance is null)
            await InitialSetup();
        else
            await ComebackSetup();
        

    }

    private async Task InitialSetup()
    {
        // Fill Dropdown with Leagues
        new LeagueService();

        var profileData = await LeagueService.Instance.GetProfileData();
        
    }

    private async Task ComebackSetup()
    {
        LeagueService.Instance.BindToEvents();
    }

    private void LeagueMain_Unloaded(object? sender, RoutedEventArgs e)
    {
        //LeagueService.Instance.UnbindToEvents();
    }
}