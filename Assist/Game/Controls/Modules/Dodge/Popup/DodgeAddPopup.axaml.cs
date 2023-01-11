using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models.Dodge;
using Assist.Game.Models.Dodge.ThirdParty;
using Assist.Game.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Modules.Dodge.Popup
{
    public partial class DodgeAddPopup : UserControl
    {
        private readonly DodgePopupViewModel _viewModel;
        public DodgeAddPopup()
        {
            DataContext = _viewModel = new DodgePopupViewModel();
            InitializeComponent();
        }

        private async void AddBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            // Get Controls
            _viewModel.Working = true;
            var gameNameBox = this.FindControl<TextBox>("GameNameBox");
            var noteBox = this.FindControl<TextBox>("NoteBox");

            if (string.IsNullOrEmpty(gameNameBox.Text))
            {
                _viewModel.ErrorMessage = "One or more Inputs are Empty";
                _viewModel.Working = false;
                btn.IsEnabled = true;
                return;
            }


            


            try
            {
                if (!ValidateGameName(gameNameBox.Text))
                {
                    _viewModel.ErrorMessage = "Gamename is not valid.";
                    _viewModel.Working = false;
                    btn.IsEnabled = true;
                    return;
                }
                var data = await _viewModel.GetUsernameInformation(gameNameBox.Text);

                if (data != null)
                {
                    var dodgeUser = new DodgeUser()
                    {
                        DateAdded = DateTime.Now,
                        GameName = $"{data.data.name}#{data.data.tag}",
                        Note = noteBox.Text,
                        UserId = data.data.puuid
                    };

                    DodgeService.Current.AddUser(dodgeUser);

                    PopupSystem.KillPopups();
                }

                _viewModel.ErrorMessage = "Failed to Get Data";
            }
            catch (Exception exception)
            {
                Log.Error("Error On Adding Player to DodgeList");
                Log.Error(exception.Message);
                _viewModel.Working = false;
                btn.IsEnabled = true;
                _viewModel.ErrorMessage = exception.Message;
            }

        }

        private bool ValidateGameName(string gameName)
        {
            var s = gameName.Split("#");
            if (!gameName.Contains("#") && !string.IsNullOrEmpty(s[0]) && !string.IsNullOrEmpty(s[1]))
                return false;

            return true;
        }

        private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            PopupSystem.KillPopups();
        }
    }

    class DodgePopupViewModel : ViewModelBase
    {
        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private bool _working = false;

        public bool Working
        {
            get => _working;
            set => this.RaiseAndSetIfChanged(ref _working, value);
        }
        public async Task<HenrikUserResponse> GetUsernameInformation(string gameName)
        {
            var t = new HttpClient();
            var name = gameName.Split("#");
            var r = await t.GetAsync($"https://api.henrikdev.xyz/valorant/v1/account/{name[0]}/{name[1]}");

            if (r == null)
            {
                throw new Exception("ERRORNULL");
            }

            if (!r.IsSuccessStatusCode)
                throw new Exception("User Not Found.");

            if (r.StatusCode == HttpStatusCode.TooManyRequests)
                throw new Exception("Server Overloaded. Please Wait.");

            var data =  JsonSerializer.Deserialize<HenrikUserResponse>(await r.Content.ReadAsStringAsync());

            return data;
        }
    }
}
