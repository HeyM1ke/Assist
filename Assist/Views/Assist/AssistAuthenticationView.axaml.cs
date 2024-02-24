using Assist.ViewModels.Assist;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Assist;

public partial class AssistAuthenticationView : UserControl
{
    private readonly AssistAuthenticationViewModel _viewModel;

    public AssistAuthenticationView()
    {
        DataContext = _viewModel = new AssistAuthenticationViewModel();
        InitializeComponent();
    }
}