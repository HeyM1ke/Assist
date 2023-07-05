using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Controls.TitleBars
{
    public partial class TitleBar_Win : UserControl
    {
        public TitleBar_Win()
        {
            InitializeComponent();
        }



        private void MinimizeBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desk) 
                desk.MainWindow.WindowState = WindowState.Minimized;
            
            Log.Information("Window was Minimized");
        }

        private void ExitBtn_Click(object? sender, RoutedEventArgs e)
        {
            AssistApplication.CurrentApplication.Shutdown();
        }

        private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (this.IsPointerOver && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desk)
                    desk.MainWindow.BeginMoveDrag(e);
            }
        }
    }
}
