using Assist.Game.Services;
using Assist.Game.Services.Leagues;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Game.Views.Leagues.Pages;

public partial class LeagueLobbyPage : UserControl
{
    public LeagueLobbyPage()
    {
        InitializeComponent();
    }



    private async void DevJoinBtn_Click(object? sender, RoutedEventArgs e)
    {
        var textInput = this.FindControl<TextBox>("PartyIDJoin");

        if (!string.IsNullOrEmpty(textInput.Text))
        {
            var r = await AssistApplication.Current.AssistUser.Party.JoinParty(textInput.Text); 
            Log.Information(r.Message);
            Log.Information("Attempting to jOin party.");
        }
    }

    private void MatchPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        GameViewNavigationController.Change(new MatchPage());
    }
}