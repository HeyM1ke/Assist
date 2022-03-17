using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using Assist.MVVM.View.InitPage;
using Assist.Settings;
using Assist.Controls;

namespace Assist.MVVM.ViewModel
{
    internal class AssistAccountSwitchViewModel
    {
        public async Task LoginIntoAccount(object obj)
        {
            await AssistApplication.AppInstance.AuthenticateWithProfileSetting(null);
        }

    }
}
