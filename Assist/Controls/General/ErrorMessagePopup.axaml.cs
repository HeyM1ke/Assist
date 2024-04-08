using System.Windows.Input;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assist.Controls.General;

public partial class ErrorMessagePopup : UserControl
{
    private readonly ErrorMessagePopupViewModel _viewModel;

    public ErrorMessagePopup()
    {
        DataContext = _viewModel = new ErrorMessagePopupViewModel();
        InitializeComponent();
        _viewModel.ErrorControlButtonCommand = _viewModel.DefaultClosePopupCommand;
    }
    
    public ErrorMessagePopup(string message)
    {
        DataContext = _viewModel = new ErrorMessagePopupViewModel();
        InitializeComponent();
        _viewModel.ErrorMessageBody = message;
        _viewModel.ErrorControlButtonCommand = _viewModel.DefaultClosePopupCommand;
    }
    
    public ErrorMessagePopup(string title, string message)
    {
        DataContext = _viewModel = new ErrorMessagePopupViewModel();
        InitializeComponent();
        _viewModel.ErrorMessageBody = message;
        _viewModel.ErrorMessageTitle = title;
        _viewModel.ErrorControlButtonCommand = _viewModel.DefaultClosePopupCommand;
    }
}

public partial class ErrorMessagePopupViewModel : ViewModelBase
{
    [ObservableProperty] private string _errorMessageTitle = "We ran into an issue.";
    [ObservableProperty] private string _errorMessageBody = "Something went wrong.";
    [ObservableProperty] private ICommand _errorControlButtonCommand;

    [RelayCommand]
    public void DefaultClosePopup()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(null);
        });
    }
}