using Assist.Game.Services;
using Avalonia.Controls;

namespace Assist.Game.Views.Live.Pages
{
    public partial class UnkownPageView : UserControl
    {
        public UnkownPageView()
        {
            LiveViewNavigationController.CurrentPage = LivePage.UNKNOWN;
            InitializeComponent();
        }
    }
}
