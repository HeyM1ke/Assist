using System;
using Assist.Game.Views.Lobbies.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Lobbies.Pages;

public partial class BrowserView : UserControl
{
    private readonly BrowserViewModel _viewModel;

    public BrowserView()
    {
        DataContext = _viewModel = new BrowserViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void BrowserView_Init(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;

        await _viewModel.Setup();
    }

    private async void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("SearchBox");
        textBox.Text = string.Empty;

        await _viewModel.RefreshList();
    }

    private async void Search_Click(object? sender, RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("SearchBox");
        await _viewModel.FilterList(textBox.Text);
    }

    private void SearchBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Search_Click(null, null);
        }
    }
}