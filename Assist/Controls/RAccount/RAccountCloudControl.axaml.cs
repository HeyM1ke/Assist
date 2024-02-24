using System;
using System.Windows.Input;
using Assist.ViewModels.RAccount;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Controls.RAccount;

public partial class RAccountCloudControl : UserControl
{
    private readonly RAccountCloudViewModel _viewModel;

    public RAccountCloudControl()
    {
        DataContext = _viewModel = new RAccountCloudViewModel();
        InitializeComponent();
    }
    
    public RAccountCloudControl(ICommand loginCompletedCommand)
    {
        DataContext = _viewModel = new RAccountCloudViewModel();
        InitializeComponent();
        _viewModel.LoginCompletedCommand = loginCompletedCommand;
    }
    

    private async void CloudWebview_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
            return; // Don't run in design mode

        await _viewModel.Setup();
    }
}