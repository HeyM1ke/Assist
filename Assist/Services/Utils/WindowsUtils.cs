using System;
using System.Runtime.InteropServices;

namespace Assist.Services.Utils;

public class WindowsUtils
{
    [DllImport ("user32.dll")]
    public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);
	
    [DllImportAttribute ("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	
    [DllImportAttribute ("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
	
    [DllImport("User32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("User32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    
    [DllImport("kernel32")]
    public static extern bool AllocConsole();
    
    private const int WS_EX_APPWINDOW = 0x40000;
    private const int WS_EX_TOOLWINDOW = 0x0080;
    private const int GWL_EXSTYLE = -0x14;
    public static void ShowToFront(string windowName)
    {
        IntPtr firstInstance = FindWindow(null, windowName);
        ShowWindow(firstInstance, 5);
        SetForegroundWindow(firstInstance);
        //SetWindowLong(firstInstance, GWL_EXSTYLE, GetWindowLong(firstInstance, WS_EX_TOOLWINDOW) |     WS_EX_TOOLWINDOW);

    }

    public static void SetWindowForeground(string windowName)
    {
        IntPtr firstInstance = FindWindow(null, windowName);
        SetForegroundWindow(firstInstance);
    }
    
    public static void HideWindow(string windowName)
    {
        IntPtr firstInstance = FindWindow(null, windowName);
        ShowWindow(firstInstance, 0);
       // SetWindowLong(firstInstance, GWL_EXSTYLE, GetWindowLong(firstInstance, GWL_EXSTYLE) |     WS_EX_TOOLWINDOW);

    }
}