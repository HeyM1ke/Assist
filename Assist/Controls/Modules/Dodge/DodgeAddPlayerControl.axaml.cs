using System;
using System.Windows.Input;
using Assist.Controls.Assist;
using Assist.Core.Helpers;
using Assist.ViewModels.Modules;
using AssistUser.Lib.V2.Models.Dodge;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void CategorySelectionChanged(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        
        var s = sender as ImageToggleButton;
        if (s.IsChecked != true) return;
        var intVal = Int32.Parse(s.Tag.ToString());
        _viewModel.PlayerSelectedCategory = Int32.Parse(s.Tag.ToString());
        _viewModel.DodgeSelectedTitle = AssistHelper.DodgeCategories[(EAssistDodgeCategory)intVal];
    }
}