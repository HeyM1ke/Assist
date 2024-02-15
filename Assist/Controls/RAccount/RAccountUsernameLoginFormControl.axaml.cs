using System.Windows.Input;
using Assist.ViewModels.RAccount;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.RAccount;

public partial class RAccountUsernameLoginFormControl : UserControl
{
    private readonly RAccountUsernameLoginViewModel _viewModel;

    public RAccountUsernameLoginFormControl()
    {
        DataContext = _viewModel = new RAccountUsernameLoginViewModel();
        InitializeComponent();
    }
    
    public RAccountUsernameLoginFormControl(ICommand loginCompletedCommand)
    {
        DataContext = _viewModel = new RAccountUsernameLoginViewModel();
        InitializeComponent();
        _viewModel.LoginCompletedCommand = loginCompletedCommand;
    }
    
    public void UpdateUsernameFieldExternal(string name) => _viewModel.UsernameText = name;
}