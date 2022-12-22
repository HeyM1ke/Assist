using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Game.Views.Live.ViewModels
{
    internal class LiveViewViewModel : ViewModelBase
    {

        private string _output = "START: ";

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }

        public async void DisplayWebsocketData()
        {
            AssistApplication.Current.RiotWebsocketService.RecieveMessageEvent += delegate(object o)
            {
                Output += $"\n{o.ToString()}";
            };
        }
    }
}
