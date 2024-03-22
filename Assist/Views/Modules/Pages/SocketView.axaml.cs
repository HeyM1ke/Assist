using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules.Pages;

public partial class SocketView : UserControl
{
    private readonly SocketViewModel _viewModel;

    public SocketView()
    {
        DataContext = _viewModel = new SocketViewModel();
        InitializeComponent();
    }
}