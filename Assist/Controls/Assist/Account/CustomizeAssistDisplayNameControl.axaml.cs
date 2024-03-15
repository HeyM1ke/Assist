using System.Windows.Input;
using Assist.ViewModels.Assist;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Assist.Account;

public partial class CustomizeAssistDisplayNameControl : UserControl
{
    private readonly CustomizeAssistDisplayNameControlViewModel _viewModel;

    public CustomizeAssistDisplayNameControl()
    {
        DataContext = _viewModel = new CustomizeAssistDisplayNameControlViewModel();
        InitializeComponent();
    }
    
    public CustomizeAssistDisplayNameControl(ICommand CompletedCommand, ICommand CloseCommand = null)
    {
        DataContext = _viewModel = new CustomizeAssistDisplayNameControlViewModel();
        InitializeComponent();
        _viewModel.FinishCommand = CompletedCommand;
        _viewModel.CloseCommand = CloseCommand;
    }
}