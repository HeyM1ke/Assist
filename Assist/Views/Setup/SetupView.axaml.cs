using Assist.ViewModels.Setup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Setup;

public partial class SetupView : UserControl
{
    private readonly SetupViewModel _viewModel;

    public SetupView()
    {
        DataContext = _viewModel = new SetupViewModel();
        InitializeComponent();
    }
}