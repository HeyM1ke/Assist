using System;
using Assist.Controls.Assist.ViewModels;
using Assist.ViewModels;
using Avalonia.Controls;

namespace Assist.Controls.Assist
{
    public partial class UserPreview : UserControl
    {
        private readonly UserPreviewViewModel _viewModel;

        public UserPreview()
        {
            DataContext = _viewModel = new UserPreviewViewModel();
            InitializeComponent();
        }


        private void UserPreview_Initialized(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            _viewModel.AssistUserName = AssistApplication.Current.AssistUser.UserInfo.username;
        }
    }
}
