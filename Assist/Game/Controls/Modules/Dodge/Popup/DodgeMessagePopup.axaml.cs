using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace Assist.Game.Controls.Modules.Dodge.Popup;

public partial class DodgeMessagePopup : UserControl
{
    private readonly DodgeMessagePopupVM _viewModel;

    public DodgeMessagePopup()
    {
        DataContext = _viewModel = new DodgeMessagePopupVM();
        InitializeComponent();
    }
    
    public DodgeMessagePopup(string message)
    {
        DataContext = _viewModel = new DodgeMessagePopupVM();
        _viewModel.Message = message;
        InitializeComponent();
    }
    
    public DodgeMessagePopup(bool isLoading)
    {
        DataContext = _viewModel = new DodgeMessagePopupVM();
        _viewModel.IsLoading = isLoading;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }
}

public class DodgeMessagePopupVM : ViewModelBase
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    
    private string _message = "";

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }
    
    
}