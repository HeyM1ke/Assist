using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models.Dodge;
using Assist.Game.Models.Dodge.ThirdParty;
using Assist.Game.Services;
using Assist.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Modules.Dodge.Popup
{
    public partial class GlobalDodgeAdd : UserControl
    {
        private readonly GlobalDodgePopupViewModel _viewModel;

        public GlobalDodgeAdd()
        {
            DataContext = _viewModel = new GlobalDodgePopupViewModel();
            InitializeComponent();
        }

        private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            PopupSystem.KillPopups();
        }

        private async void AddBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            // Get Controls
            _viewModel.Working = true;
            var gameNameBox = this.FindControl<TextBox>("GameNameBox");
            var categoryBox = this.FindControl<ComboBox>("CategoryBox");

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

                    var selected = categoryBox.SelectedItem as ComboBoxItem;

                   var r = await AssistApplication.Current.AssistUser.AddGlobalDodgeList(new GlobalDodgeUser()
                    {
                        id = data.data.puuid,
                        category = selected.Content.ToString()
                    });

                   if (r != null)
                   {
                       _viewModel.ErrorMessage = r.message;
                       _viewModel.Working = false;
                       btn.IsEnabled = true;
                    }
                }
                else
                {
                    _viewModel.ErrorMessage = "Failed to Get Data";
                    return;
                }

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
    }

    class GlobalDodgePopupViewModel : ViewModelBase
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

            var data = JsonSerializer.Deserialize<HenrikUserResponse>(await r.Content.ReadAsStringAsync());

            return data;
        }
    }
}
