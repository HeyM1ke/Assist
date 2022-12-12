using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Assist.Services;

namespace Assist.Views.Store
{
    public partial class StoreView : UserControl
    {
        public StoreView()
        {
            InitializeComponent();
            MainViewNavigationController.CurrentPage = Page.STORE;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
