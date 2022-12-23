using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Game.Controls.Modules.Dodge;
using Assist.Game.Models.Dodge;
using Assist.Game.Services;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using ReactiveUI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assist.Game.Views.Modules.Views.ViewModels
{
    internal class DodgeViewViewModel : ViewModelBase
    {
        private List<DodgeUserButton> _userButtons = new List<DodgeUserButton>();
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }
        public List<DodgeUserButton> UserButtons
        {
            get => _userButtons;
            set => this.RaiseAndSetIfChanged(ref _userButtons, value);
        }


        public async Task LoadUsers()
        {
            if(Design.IsDesignMode)
                return;

            DodgeService.Current.DodgeUserAddedToList += DodgeUserAddedToList;
            DodgeService.Current.DodgeUserRemovedFromList += DodgeUserRemovedFromList;
            for (int i = 0; i < DodgeService.Current.UserList.Count; i++)
            {
                var btn = new DodgeUserButton()
                {
                    GameName = DodgeService.Current.UserList[i].GameName,
                    Note = DodgeService.Current.UserList[i].Note,
                    DateAdded = $"Date Added: {DodgeService.Current.UserList[i].DateAdded.ToShortDateString()}"

                };
                btn.Click += DodgeUserButton_Click;
                UserButtons.Add(btn);
            }
        }

        private void DodgeUserButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var btn = sender as DodgeUserButton;
            DodgeService.Current.RemoveUser(DodgeService.Current.UserList.Find(user => user.GameName == btn.GameName));
        }

        private void DodgeUserRemovedFromList(DodgeUser user)
        {
            var t = UserButtons.ToList();
            t.Remove(t.Find(button => button.GameName == user.GameName));
            UserButtons = t;
        }

        private void DodgeUserAddedToList(DodgeUser user)
        {
            var btn = new DodgeUserButton()
            {
                GameName = user.GameName,
                Note = user.Note,
                DateAdded = $"Date Added: {user.DateAdded.ToShortDateString()}"
            };

            btn.Click += DodgeUserButton_Click;

            var t = new List<DodgeUserButton>()
            {
                btn
            };

            var newList = UserButtons.Concat(t).ToList();

            UserButtons = newList;
        }
    }
}
