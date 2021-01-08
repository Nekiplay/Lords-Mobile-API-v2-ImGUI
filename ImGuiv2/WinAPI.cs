using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ImGuiv2
{
    public class WinAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
