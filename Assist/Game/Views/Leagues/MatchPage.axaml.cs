using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues;

public partial class MatchPage : UserControl
{
    public MatchPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MatchPage_Loaded(object? sender, RoutedEventArgs e)
    {
        // Get Match Data
    }
}