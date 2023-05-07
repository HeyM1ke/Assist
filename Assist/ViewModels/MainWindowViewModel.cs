using System;
using System.Collections.Generic;
using System.Text;
using Assist.Objects.Enums;
using Assist.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Assist.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public UserControl CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        private UserControl _currentView;

        private double _scaleRate = 1.0;

        public double ScaleRate
        {
            get => _scaleRate;
            set => this.RaiseAndSetIfChanged(ref _scaleRate, value);
        }

        private int _height = 720;

        public int Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        private int _width = 1190;

        public int Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        public void DetermineScaleRate()
        {
            ChangeResolution(AssistSettings.Current.SelectedResolution);
        }


        public void ChangeResolution(EResolution Res)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;

                if (mainRef.Screens.Primary.WorkingArea.Height <= 1080 && (int)Res >= 2 )
                {
                    AssistSettings.Current.SelectedResolution = EResolution.R900;
                    Res = AssistSettings.Current.SelectedResolution;
                }

                switch (Res)
                {
                    case EResolution.R360:
                        Width = 595;
                        Height = 375;
                        ScaleRate = .5;
                        break;
                    case EResolution.R540:
                        Width = 893;
                        Height = 550;
                        ScaleRate = .75;
                        break;
                    case EResolution.R900:
                        Width = 1488;
                        Height = 890;
                        ScaleRate = 1.25;
                        break;
                    case EResolution.R1080:
                        Width = 1785;
                        Height = 1060;
                        ScaleRate = 1.5;
                        break;
                    case EResolution.R1260:
                        Width = 2083;
                        Height = 1240;
                        ScaleRate = 1.75;
                        break;
                    default:
                        Width = 1190;
                        Height = 720;
                        ScaleRate = 1.0;
                        break;
                }

            }
        }
    }
}
