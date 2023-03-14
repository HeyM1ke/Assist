using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Chat;

public partial class AssistChatWindow : UserControl
{
    
    public string GroupId = String.Empty;
    public AssistChatWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public AssistChatWindow(string groupID)
    {
        InitializeComponent();
        
    }
}