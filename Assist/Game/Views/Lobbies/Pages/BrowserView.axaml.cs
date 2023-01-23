using Assist.Game.Views.Lobbies.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Lobbies.Pages;

public partial class BrowserView : UserControl
{
    private readonly BrowserViewModel _viewModel;

    public BrowserView()
    {
        DataContext = _viewModel = new BrowserViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}