using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.Modules.Views;
using Avalonia.Controls;

namespace Assist.Game.Views.Modules
{
    public partial class ModulesView : UserControl
    {
        public ModulesView()
        {
            GameViewNavigationController.CurrentPage = Page.MODULES;
            InitializeComponent();
            ModulesViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentControl");
            ModulesViewNavigationController.Change(new SelectionView());
        }
    }
}
