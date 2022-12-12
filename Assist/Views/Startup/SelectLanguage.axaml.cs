using System;
using System.Collections.Generic;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Startup
{
    public partial class SelectLanguage : UserControl
    {
        ComboBox _languageComboBox;
        public SelectLanguage()
        {
            InitializeComponent();
            _languageComboBox = this.FindControl<ComboBox>("LangComboBox");
            _languageComboBox.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ApplyBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            AssistSettings.Current.Language = (ELanguage) _languageComboBox.SelectedIndex;
            App.ChangeLanguage();
            AssistSettings.Current.LanguageSelected = true;
            AssistSettings.Save();
            MainWindowContentController.Change(new InitialScreen());
        }

        private void SelectLanguage_OnInitialized(object? sender, EventArgs e)
        {

        }
    }
}
