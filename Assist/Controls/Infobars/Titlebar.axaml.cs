using Assist.Models.Enums;
using Assist.Shared.Services.Utils;
using Assist.ViewModels;
using Assist.ViewModels.Infobars;
using Assist.Views.ProfileSwap;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.Controls.Infobars;

public partial class Titlebar : UserControl
{
    public static TitlebarViewModel ViewModel { get; set; }
    public Titlebar()
    {
        DataContext = ViewModel = new TitlebarViewModel();
        InitializeComponent();
    }
    
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (this.IsPointerOver && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desk)
                desk.MainWindow.BeginMoveDrag(e);
        }
    }

    [RelayCommand]
    private void ExitAssist()
    {
        AssistApplication.RequestedClose = true;

        if (AssistApplication.CurrentlyInAssistLeagueMatch)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Log.Information("Program is currently in League match. Hiding Program then closing it after match is completed.");
                WindowsUtils.HideWindow("Assist");
                return;
            }
        }
            
        AssistApplication.CurrentApplication.Shutdown();
    }


    [RelayCommand]
    private void MinimizeAssist()
    {
        if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desk) 
            desk.MainWindow.WindowState = WindowState.Minimized;
            
        Log.Information("Window was Minimized");
    }

    private void ExitBtn_Click(object? sender, RoutedEventArgs e)
    {
        AssistApplication.RequestedClose = true;

        if (AssistApplication.CurrentlyInAssistLeagueMatch)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Log.Information("Program is currently in League match. Hiding Program then closing it after match is completed.");
                WindowsUtils.HideWindow("Assist");
                return;
            }
        }
            
        AssistApplication.CurrentApplication.Shutdown();
    }

    private void MinimizeBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desk) 
            desk.MainWindow.WindowState = WindowState.Minimized;
            
        Log.Information("Window was Minimized");
    }
    
}