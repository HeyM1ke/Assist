using System.Windows.Input;
using Assist.ViewModels.Assist;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Assist.Authentication;

public partial class AssistAccountLoginForm : UserControl
{
    private readonly AssistAccountLoginFormViewModel _viewModel;

    public AssistAccountLoginForm()
    {
        DataContext = _viewModel = new AssistAccountLoginFormViewModel();
        InitializeComponent();
    }

    public AssistAccountLoginForm(ICommand registerCommand, ICommand accountComplete = null)
    {
        DataContext = _viewModel = new AssistAccountLoginFormViewModel();
        InitializeComponent();
        _viewModel.BackToRegisterCommand = registerCommand;
        _viewModel.AccountCompleteCommand = accountComplete;
    }
}