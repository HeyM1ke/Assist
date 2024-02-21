using Assist.Game.Controls.Live.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Live;

public partial class MatchReportSlideout : UserControl
{
    private readonly MatchReportSlideoutViewModel _viewModel;

    public MatchReportSlideout()
    {
        DataContext = _viewModel = new MatchReportSlideoutViewModel();
        InitializeComponent();
    }

    private void SlideBtn_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.SlideOpen = !_viewModel.SlideOpen;
        (sender as Button).Content = _viewModel.SlideOpen ? ">" : "<";

        /*if (_viewModel.SlideOpen)
            _viewModel.RefreshList();*/
    }

    private void ReportSlideOut_Loaded(object? sender, RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
            _viewModel.LoadMatches();
    }
}