using System;
using Assist.Controls.Assist;
using Assist.Core.Helpers;
using Assist.ViewModels.Modules;
using AssistUser.Lib.V2.Models.Dodge;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Modules.Dodge;

public partial class DodgeQuickAddPlayerControl : UserControl
{
    private readonly DodgeQuickAddPlayerViewModel _viewModel;

    public DodgeQuickAddPlayerControl()
    {
        InitializeComponent();
    }
    
    public DodgeQuickAddPlayerControl(string idToAdd)
    {
        DataContext = _viewModel = new DodgeQuickAddPlayerViewModel();
        InitializeComponent();
        _viewModel.PlayerId = idToAdd;
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