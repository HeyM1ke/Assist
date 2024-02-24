using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules;

public partial class ModulesView : UserControl
{
    private readonly ModulesViewModel _viewModel;

    public ModulesView()
    {
        DataContext = _viewModel = new ModulesViewModel();
        InitializeComponent();
    }
}