using Assist.Game.Services;
using Avalonia.Controls;

namespace Assist.Game.Views.Live.Pages
{
    public partial class MenusPageView : UserControl
    {
        public MenusPageView()
        {
            LiveViewNavigationController.CurrentPage = LivePage.MENUS;
            InitializeComponent();
        }
    }
}
