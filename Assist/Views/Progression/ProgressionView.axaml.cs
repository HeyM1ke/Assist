using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Assist.Services;

namespace Assist.Views.Progression
{
    public partial class ProgressionView : UserControl
    {
        public ProgressionView()
        {
            InitializeComponent();
            MainViewNavigationController.CurrentPage = Page.PROGRESS;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
