using System.Windows.Input;
using Assist.ViewModels.RAccount;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.RAccount;

public partial class RAccountSecondaryClientLoginControl : UserControl
{
    private readonly RAccountSecondaryClientLoginViewModel _viewModel;

    public RAccountSecondaryClientLoginControl()
    {
        DataContext = _viewModel = new RAccountSecondaryClientLoginViewModel();
        InitializeComponent();
    }
    
    public RAccountSecondaryClientLoginControl(ICommand _loginCompletedCommand)
    {
        DataContext = _viewModel = new RAccountSecondaryClientLoginViewModel();
        InitializeComponent();
        _viewModel.LoginCompletedCommand = _loginCompletedCommand;
    }
}