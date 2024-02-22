using Assist.ViewModels.RAccount;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.RAccount;

public partial class RAccountAddPage : UserControl
{
    private readonly RAccountAddViewModel _viewModel;

    public RAccountAddPage()
    {
        DataContext = _viewModel = new RAccountAddViewModel();
        InitializeComponent();
    }
    
    public RAccountAddPage(string? username)
    {
        DataContext = _viewModel = new RAccountAddViewModel(username);
        InitializeComponent();
        
    }
    
    public RAccountAddPage(bool skipInital)
    {
        DataContext = _viewModel = new RAccountAddViewModel(skipInital);
        InitializeComponent();
        
    }
}