using Assist.Controls.Global.ViewModels;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global
{
    public partial class UserSelectionBtn : UserControl
    {
        private readonly UserSelectBtnViewModel _viewModel;

        public UserSelectionBtn()
        {   
            DataContext = _viewModel = new UserSelectBtnViewModel();
            InitializeComponent();
        }
        
        public UserSelectionBtn(ProfileSettings pro)
        {   
            DataContext = _viewModel = new UserSelectBtnViewModel();
            InitializeComponent();
            _viewModel.Profile = pro;
        }



        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            AssistApplication.Current.SwapCurrentProfile(_viewModel.Profile);
            btn.IsEnabled = !false;
        }
    }
}