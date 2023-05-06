using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views.Authentication;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

namespace Assist.Controls.Global.ViewModels;

public class AccountManagementViewModel : ViewModelBase
{
    private ObservableCollection<Control> _accountItems = new ObservableCollection<Control>();

    public ObservableCollection<Control> AccountItems
    {
        get => _accountItems;
        set => this.RaiseAndSetIfChanged(ref _accountItems, value);
    }

    public async Task Setup()
    {
        foreach (var profile in AssistSettings.Current.Profiles)
        {
            var btn = new AccountManagementUserButton(profile);
            AccountItems.Add(btn);
        }

        var addAccbtn = new AccountManagementAddButton();
        
        addAccbtn.Click += AddAccbtnOnClick;
        
        AccountItems.Add(addAccbtn);
    }

    private void AddAccbtnOnClick(object? sender, RoutedEventArgs e)
    {
        MainWindowContentController.Change(new AuthenticationView());
        PopupSystem.KillPopups();
    }
}