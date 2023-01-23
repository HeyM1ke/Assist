using Assist.Game.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Lobbies;

public partial class LobbiesView : UserControl
{
    public LobbiesView()
    {
        GameViewNavigationController.CurrentPage = Page.LOBBIES;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}