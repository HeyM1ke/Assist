using Assist.Game.Services;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Assist.Game.Views.Modules.Views
{
    public partial class SelectionView : UserControl
    {
        private readonly SelectionViewVM _viewModel;

        public SelectionView()
        {
            ModulesViewNavigationController.CurrentPage = ModulePage.SELECTION;
            DataContext = _viewModel = new SelectionViewVM();
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

    public class SelectionViewVM : ViewModelBase
    {
        public bool IsGameMode => AssistApplication.Current.Mode == AssistMode.GAME;
    }
}
