using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.ViewModels;
using Avalonia.Controls;
using ReactiveUI;

namespace Assist.Game.Controls.Chat.ViewModels;

public class AssistChatboxViewModel : ViewModelBase
{
    private ObservableCollection<TabItem> _tabItems = new ObservableCollection<TabItem>();

    public ObservableCollection<TabItem> TabItems
    {
        get => _tabItems;
        set => this.RaiseAndSetIfChanged(ref _tabItems, value);
    }

    public async Task AddChat(string name, string groupId)
    {
        var chatWindow = new AssistChatWindow();
        var cha2tWindow = new AssistChatWindow();

        var tbItem = new TabItem()
        {
            Header = name,
            IsEnabled = true,
            Content = chatWindow
        };
        TabItems.Add(tbItem);
         tbItem = new TabItem()
        {
            Header = "FK",
            IsEnabled = true,
            Content = cha2tWindow
        };

        TabItems.Add(tbItem);
    }
}   