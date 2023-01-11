using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Controls.Assist.ViewModels
{
    internal class UserPreviewViewModel : ViewModelBase
    {
        private string _assistUserName;

        public string AssistUserName
        {
            get => _assistUserName;
            set => this.RaiseAndSetIfChanged(ref _assistUserName, value);
        }
    }
}
