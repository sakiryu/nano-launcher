using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NanoLauncher
{
    public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

    public enum Event : int
    {
        None = 0x0,
        MouseMove = 0x0200,
        MouseLeave = 0x02A3,
        LeftButtonDown = 0x0201,
        LeftButtonUp = 0x0202,
        NclButtonDown = 0xA1,
        Caption = 0x2
    }

    public static class Native
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    }
}
