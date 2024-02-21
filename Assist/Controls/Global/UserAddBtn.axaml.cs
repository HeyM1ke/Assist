using Assist.Services;
using Assist.Views.Authentication;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global
{
    public partial class UserAddBtn : UserControl
    {
        public UserAddBtn()
        {
            InitializeComponent();
        }



        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            MainWindowContentController.Change(new AuthenticationView());
        }
    }
}