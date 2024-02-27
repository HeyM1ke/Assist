using System.Windows.Input;
using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Modules.Dodge;

public partial class DodgeAddPlayerControl : UserControl
{
    private readonly DodgeAddPlayerViewModel _viewModel;

    public DodgeAddPlayerControl()
    {
        DataContext = _viewModel = new DodgeAddPlayerViewModel();
        InitializeComponent();
    }
    
    public DodgeAddPlayerControl(ICommand CloseAddControlCommand)
    {
        DataContext = _viewModel = new DodgeAddPlayerViewModel();
        InitializeComponent();
        _viewModel.CloseViewCommand = CloseAddControlCommand;
    }
}