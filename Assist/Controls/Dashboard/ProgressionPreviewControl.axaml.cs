using Assist.ViewModels.Dashboard;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard;

public partial class ProgressionPreviewControl : UserControl
{
    private readonly ProgressionPreviewViewModel _viewModel;

    public ProgressionPreviewControl()
    {
        DataContext = _viewModel = new ProgressionPreviewViewModel();
        InitializeComponent();
    }
}