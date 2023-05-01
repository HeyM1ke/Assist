using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global.Navigation;

public partial class LauncherVerticalNavigationBar : UserControl
{
    public LauncherVerticalNavigationBar()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}