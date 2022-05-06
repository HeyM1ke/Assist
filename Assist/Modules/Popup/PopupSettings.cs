using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Modules.Popup
{
    public class PopupSettings
    {
        public string PopupTitle { get; set; }
        public string PopupDescription { get; set; }

        public PopupType PopupType { get; set; }
    }

    public enum PopupType
    {
        OK,
        LOADING,
        ERROR
    }
}

