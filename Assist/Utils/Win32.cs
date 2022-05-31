using System.Runtime.InteropServices;

namespace Assist.Utils;

public static class Win32
{

    [DllImport("kernel32")]
    public static extern bool AllocConsole();

}
