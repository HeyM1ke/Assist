using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Objects.AssistApi.Server;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace Assist.Game.Controls.GDashboard.ViewModels
{
    internal class GlobalChatBoxViewModel : ViewModelBase
    {
        private List<GlobalChatMessageControl> _messageControls = new List<GlobalChatMessageControl>();

        public List<GlobalChatMessageControl> MessageControls
        {
            get => _messageControls;
            set => this.RaiseAndSetIfChanged(ref _messageControls, value);
        }


        public async void AddMessage(ServerChatMessage chatMessageData)
        {

            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var temp = new List<GlobalChatMessageControl>
                {
                    new GlobalChatMessageControl()
                    {
                        Message = chatMessageData.Message,
                        Username = chatMessageData.Username,
                        TimeStamp = chatMessageData.TimeSent.ToShortTimeString()
                    }
                };

                MessageControls = MessageControls.Concat(temp).ToList();

            });
        }

        private void AddToControls(GlobalChatMessageControl control)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var temp = new List<GlobalChatMessageControl>
                {
                    control
                };
                MessageControls = MessageControls.Concat(temp).ToList();
            });
        }

        public async void SendMessage(string messageText)
        {
            await AssistApplication.Current.GameServerConnection.SendGlobalChatMessage(messageText);
        }
    }
}
