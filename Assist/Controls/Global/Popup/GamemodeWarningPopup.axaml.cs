using System;
using Assist.Game.Views.Initial;
using Assist.Services;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Controls.Global.Popup
{
    public partial class GamemodeWarningPopup : UserControl
    {
        public Action WarningClosing;
        public GamemodeWarningPopup()
        {
            InitializeComponent();
        }

        // On Button Click to go to game mode
        // Just go to gamemode

        // if not to go to gamemode
        // invoke the warning close and kill popups from initial.


        private void NoBtn_Click(object? sender, RoutedEventArgs e)
        {
            WarningClosing?.Invoke();

        }

        private void YesBtn_Click(object? sender, RoutedEventArgs e)
        {
            MainWindowContentController.Change(new GameInitialView());
            return;
        }
    }
}
