using Avalonia.Controls;
using DynamicData;
using System.Collections.Generic;
using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.Modules;
using Assist.Game.Views.Modules.Views;
using Avalonia.Input;
using Assist.Services;
using Assist.Views.Dashboard;


namespace Assist.Game.Controls.Navigation
{
    public partial class GameNavigationBar : UserControl
    {
        public static GameNavigationBar Instance;
        public List<GameNavigationButton> NavigationButtons = new List<GameNavigationButton>();
        public GameNavigationBar()
        {
            InitializeComponent();
            NavigationButtons.Add(this.FindControl<GameNavigationButton>("LiveBtn"));
            NavigationButtons.Add(this.FindControl<GameNavigationButton>("ModulesBtn"));
            Instance = this;

        }

        private void LiveBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LIVE)
                GameViewNavigationController.Change(new LiveView());

            NavigationButtons[0].IsSelected = true;
        }

        private void ClearSelected()
        {
            NavigationButtons.ForEach(btn => btn.IsSelected = false);
        }

        private void ModulesBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.MODULES)
                GameViewNavigationController.Change(new ModulesView());

            NavigationButtons[1].IsSelected = true;
        }
    }
}
