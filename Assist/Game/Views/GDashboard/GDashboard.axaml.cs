using Assist.Game.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.GDashboard;

public partial class GDashboard : UserControl
{
    public GDashboard()
    {

        GameViewNavigationController.CurrentPage = Page.DASHBOARD;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}