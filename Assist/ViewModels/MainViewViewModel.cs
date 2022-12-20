using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Assist.ViewModels
{
    internal class MainViewViewModel : ViewModelBase
    {
        private string? _userCount;

        public string? UserCount
        {
            get => _userCount;
            set => this.RaiseAndSetIfChanged(ref _userCount, value);
        }

        private bool? _isConnected = false;
        public bool? IsConnected
        {
            get => _isConnected;
            set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }

        public void SetupUserCount()
        {
            AssistApplication.Current.ServerHub.RecieveMessageEvent += o =>
            {
                var number = o is int ? (int)o : (int?)null;

                if (number != null)
                {
                    IsConnected = true;
                    UserCount = $"{number}";
                }
            };
        }
    }
}
