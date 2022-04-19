using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.MVVM.ViewModel;

namespace Assist.MVVM.View.Extra.ViewModels
{
    internal class UpdateWindowViewModel : ViewModelBase
    {
        private string _version;

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        private string _changeLog;

        public string ChangeLog
        {
            get => _changeLog;
            set => SetProperty(ref _changeLog, value);
        }
    }
}
