using Assist.Game.Views.LinkRiot.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.LinkRiot;

public partial class LinkRiotPopupView : UserControl
{
    private readonly LinkRiotViewModel _viewModel;

    public LinkRiotPopupView()
    {
        DataContext = _viewModel = new LinkRiotViewModel();
        InitializeComponent();
    }



    private async void LinkBtn_Click(object? sender, RoutedEventArgs e)
    {
        (sender as Button).IsEnabled = false;
        await _viewModel.LinkRiotAccountWithCurrentSocketAccount();
        (sender as Button).IsEnabled = true;
    }

    private async void RiotLink_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        _viewModel.Setup();
    }
}