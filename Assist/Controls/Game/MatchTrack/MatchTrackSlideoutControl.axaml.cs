using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Game.MatchTrack;

public partial class MatchTrackSlideoutControl : UserControl
{
    private readonly MatchTrackSlideoutViewModel _viewModel;

    public MatchTrackSlideoutControl()
    {
        DataContext = _viewModel = new MatchTrackSlideoutViewModel();
        InitializeComponent();
    }

    private void MatchTrackSlideout_Loaded(object? sender, RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
            _viewModel.LoadMatches();
    }

    private void MatchTrackSlideout_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
            _viewModel.UnloadPage();
    }
}