using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.Modules.Views;
using Assist.Services;
using Avalonia.Controls;
using Page = Assist.Game.Services.Page;

namespace Assist.Game.Views.Modules
{
    public partial class ModulesView : UserControl
    {
        public ModulesView()
        {
            GameViewNavigationController.CurrentPage = Page.MODULES;
            MainViewNavigationController.CurrentPage = Assist.Services.Page.MODULES;
            InitializeComponent();
            ModulesViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentControl");
            ModulesViewNavigationController.Change(new SelectionView());
        }
    }
}
