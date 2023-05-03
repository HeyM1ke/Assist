using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Modules.Dodge;
using Assist.Game.Controls.Modules.Dodge.Popup;
using Assist.Game.Models.Dodge;
using Assist.Game.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using ReactiveUI;
using Serilog;
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

        private bool _isGlobalEnabled = false;

        public bool IsGlobalEnabled
        {
            get => _isGlobalEnabled;
            set => this.RaiseAndSetIfChanged(ref _isGlobalEnabled, value);
        }

        private bool _isGlobalModEnabled = false;

        public bool IsGlobalModEnabled
        {
            get => _isGlobalModEnabled;
            set => this.RaiseAndSetIfChanged(ref _isGlobalModEnabled, value);
        }
        public async Task LoadUsers()
        {
            if(Design.IsDesignMode)
                return;

            new DodgeService();
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

            IsGlobalEnabled = GameSettings.Current.GlobalListEnabled;
            IsGlobalModEnabled = await AssistApplication.Current.AssistUser.Dodge.CheckGlobalDodgeList();
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

        public void RemoveAllDodgeUsers()
        {
            DodgeService.Current.UserList.Clear();
            DodgeService.SaveSettings();
            
            // Update UI
            UserButtons = new List<DodgeUserButton>();
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

        public async Task ExportDodgeList()
        {
            // Export Dodge List
            // Open File Dialog to save file somewhere
            var saveFileDialog = new SaveFileDialog()
            {
                Title = "Select where to save DodgeList File",
                DefaultExtension = ".assdge",
            };
            
            saveFileDialog.Filters.Add(new FileDialogFilter()
            {
                Extensions = new List<string>()
                {
                    "assdge"    
                }
            });
            
            var result = await saveFileDialog.ShowAsync(AssistApplication.CurrentApplication.MainWindow);
            if (result is null || result.Length == 0)
                return;
            Log.Information($"Save DodgeFile to {result}");

            var fileContent = JsonSerializer.Serialize(DodgeService.Current.UserList);
            var data = await FromForFunConvertionTo64(fileContent);
            File.WriteAllText(result, data);
            
            PopupSystem.SpawnCustomPopup(new DodgeMessagePopup($"Successfully exported list."));
        }
        
        public async Task ImportDodgeList()
        {
            // Export Dodge List
            // Open File Dialog to save file somewhere
            
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select a DodgeList File",
                AllowMultiple = false
            };
            
            openFileDialog.Filters.Add(new FileDialogFilter()
            {
                Extensions = new List<string>()
                {
                    "assdge"
                }
            });
            
            var result = await openFileDialog.ShowAsync(AssistApplication.CurrentApplication.MainWindow);
            if (result is null || result.Length == 0)
                return;
            
            Log.Information($"Importing DodgeFile from {result[0]}");
            
            Log.Information("Attempting to Read Dodge File");
            var importContent = File.ReadAllText(result[0]);
            List<DodgeUser> usersToAdd = new List<DodgeUser>();
            try
            {
                var data = await From64ToForFunConvertion(importContent);
                usersToAdd = JsonSerializer.Deserialize<List<DodgeUser>>(data);
            }
            catch (Exception e)
            {
                Log.Error("Failed to read import file.");
                
                // Show Error Popup saying that we failed to read the file.
                PopupSystem.SpawnCustomPopup(new DodgeMessagePopup("Failed to read the file."));
                return;
            }
            Log.Information("Valid Dodge File Read");
            int numberOfUsersAdded = 0;
            for (int i = 0; i < usersToAdd.Count; i++)
            {
                var exists = DodgeService.Current.UserList.Exists(user => user.UserId == usersToAdd[i].UserId);
                if (exists)
                {
                    Log.Information("Existing User found, Skipping.");
                    return;
                }
                usersToAdd[i].DateAdded = DateTime.Now;
                
                DodgeService.Current.AddUser(usersToAdd[i]);
                numberOfUsersAdded++;
            }
            Log.Information("Added all unique users in list.");
            PopupSystem.SpawnCustomPopup(new DodgeMessagePopup($"Added {numberOfUsersAdded} Users."));
        }

        private async Task<string> FromForFunConvertionTo64(string data)
        {
            var textBytes = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(textBytes);
        }
        
        private async Task<string> From64ToForFunConvertion(string data)
        {
            var textBytes = Convert.FromBase64String(data);
            var t = Encoding.UTF8.GetString(textBytes);
            return t;
        }
    }
}
