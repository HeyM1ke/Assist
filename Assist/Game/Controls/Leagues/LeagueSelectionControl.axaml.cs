using Assist.Game.Controls.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Leagues;

public partial class LeagueSelectionControl : UserControl
{
    private readonly LeagueSelectionViewModel _viewModel;

    public LeagueSelectionControl()
    {
        DataContext = _viewModel = new LeagueSelectionViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void NameBtn_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.PopupOpen = true;
    }

    private void LeagueSelectionPopup_OnPointerExited(object? sender, PointerEventArgs e)
    {
        _viewModel.PopupOpen = false;
    }
}