using System;
using Assist.Controls.Global;
using Avalonia.Controls;

namespace Assist.Views.Development
{
    public partial class Dev_Test : UserControl
    {
        public Dev_Test()
        {
            InitializeComponent();
        }

        private void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
            var o = sender as AssistImage;

            if (o != null)
            {
                o.LoadImage();
            }
        }
    }
}
