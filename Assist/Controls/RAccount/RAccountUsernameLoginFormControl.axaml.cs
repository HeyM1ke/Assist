using System.Windows.Input;
using Assist.ViewModels.RAccount;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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

    private void PasswordBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;
        
        _viewModel.LoginButtonPressedCommand.Execute(null);
    }

    private void UsernameLoginForm_Loaded(object? sender, RoutedEventArgs e)
    {
        
    }
}