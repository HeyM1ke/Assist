using Assist.Game.Services.Leagues;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues.Pages;

public partial class LeagueFinderPage : UserControl
{
    public LeagueFinderPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}