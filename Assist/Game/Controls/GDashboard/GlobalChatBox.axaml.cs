using System.Text.Json;
using Assist.Game.Controls.GDashboard.ViewModels;
using Assist.Objects.AssistApi.Server;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Assist.Game.Controls.GDashboard
{
    public partial class GlobalChatBox : UserControl
    {
        private readonly GlobalChatBoxViewModel _viewModel;

        public GlobalChatBox()
        {
            DataContext = _viewModel = new GlobalChatBoxViewModel();
            InitializeComponent();
        }

        private async void SendMessageBtn_Click(object? sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var messageTextBox = this.GetControl<TextBox>("ChatMessageTextBox");

            if (string.IsNullOrEmpty(messageTextBox.Text))
            {
                (sender as Button).IsEnabled = true;
                return;
            }
                

            
            _viewModel.SendMessage(messageTextBox.Text);
            messageTextBox.Text = string.Empty;
            (sender as Button).IsEnabled = true;
        }

        private void GlobalChatBox_Loaded(object? sender, RoutedEventArgs e)
        {
            AssistApplication.Current.GameServerConnection.GLOBALCHAT_MessageReceived += GameServerConnectionOnGLOBALCHAT_MessageReceived;
        }

        private void GameServerConnectionOnGLOBALCHAT_MessageReceived(string? obj)
        {
            // What to do when a message is received.

            if (obj == null)
                return;

            _viewModel.AddMessage(JsonSerializer.Deserialize<ServerChatMessage>(obj));

        }

        private void GlobalChatBox_Unloaded(object? sender, RoutedEventArgs e)
        {
            AssistApplication.Current.GameServerConnection.GLOBALCHAT_MessageReceived -= GameServerConnectionOnGLOBALCHAT_MessageReceived;
        }

        private void ChatMessageTextBox_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessageBtn_Click(this.GetControl<Button>("SendMessageBtn"), null);
            }
        }
    }
}
