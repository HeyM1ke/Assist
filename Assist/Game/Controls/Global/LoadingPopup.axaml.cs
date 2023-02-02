using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Global;

public partial class LoadingPopup : UserControl
{
    public LoadingPopup()
    {
        InitializeComponent();
    }
    

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}