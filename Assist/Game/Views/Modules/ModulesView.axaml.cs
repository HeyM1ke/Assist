using Assist.Game.Services;
using Avalonia.Controls;

namespace Assist.Game.Views.Modules
{
    public partial class ModulesView : UserControl
    {
        public ModulesView()
        {
            GameViewNavigationController.CurrentPage = Page.MODULES;
            InitializeComponent();
        }
    }
}
