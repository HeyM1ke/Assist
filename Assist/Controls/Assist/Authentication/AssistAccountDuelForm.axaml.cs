using System.Windows.Input;
using Assist.Controls.Setup;
using Assist.ViewModels.Assist;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;

namespace Assist.Controls.Assist.Authentication;

public partial class AssistAccountDuelForm : UserControl
{
    private readonly AssistAccountDuelFormViewModel _viewModel;
    
    public AssistAccountDuelForm()
    {
        DataContext = _viewModel = new AssistAccountDuelFormViewModel();
        InitializeComponent();
    }
    
    public AssistAccountDuelForm(ICommand OpenLoginPanel, ICommand accountComplete = null)
    {
        DataContext = _viewModel = new AssistAccountDuelFormViewModel();
        InitializeComponent();

        _viewModel.LoginSelectionCommand = OpenLoginPanel;
        _viewModel.AccountCompleteCommand = accountComplete;
    }
    
}