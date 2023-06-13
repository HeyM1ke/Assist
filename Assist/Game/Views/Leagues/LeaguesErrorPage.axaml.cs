using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues;

public partial class LeaguesErrorPage : UserControl
{
    public LeaguesErrorPage()
    {
        InitializeComponent();
    }

    public LeaguesErrorPage(string message)
    {
        InitializeComponent();
        this.FindControl<TextBlock>("ErrorBox").Text = message;
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}