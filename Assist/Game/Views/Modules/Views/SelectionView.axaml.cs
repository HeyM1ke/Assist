using Assist.Game.Services;
using Avalonia.Controls;
using Avalonia.Input;

namespace Assist.Game.Views.Modules.Views
{
    public partial class SelectionView : UserControl
    {
        public SelectionView()
        {
            ModulesViewNavigationController.CurrentPage = ModulePage.SELECTION;
            InitializeComponent();
        }

        private void DodgeModule_Press(object? sender, PointerPressedEventArgs e)
        {
            ModulesViewNavigationController.Change(new DodgeView());
        }

        private void RichPresence_Press(object? sender, PointerPressedEventArgs e)
        {
            ModulesViewNavigationController.Change(new RichPresenceView());
        }
    }
}
