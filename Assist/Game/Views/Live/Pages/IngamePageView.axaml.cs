using Assist.Game.Services;
using Avalonia.Controls;

namespace Assist.Game.Views.Live.Pages
{
    public partial class IngamePageView : UserControl
    {
        public IngamePageView()
        {
            LiveViewNavigationController.CurrentPage = LivePage.INGAME;
            InitializeComponent();
        }
    }
}
