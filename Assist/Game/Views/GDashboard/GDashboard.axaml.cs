using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.GDashboard;

public partial class GDashboard : UserControl
{
    public GDashboard()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}