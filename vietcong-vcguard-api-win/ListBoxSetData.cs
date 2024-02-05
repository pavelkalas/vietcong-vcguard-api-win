using System;
using System.Runtime.InteropServices;
using System.Text;

namespace vietcong_vcguard_api_win
{
    internal class ListBoxSetData
    {
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] static extern int GetWindowTextLength(IntPtr hWnd);

        private readonly int LB_GETTEXT = 0x0189;
        private readonly int LB_ADDSTRING = 0x0180;

        /// <summary>
        /// Sets vcguard listbox (console) data.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        public void SetData(IntPtr hWnd, string text)
        {
            if (hWnd != IntPtr.Zero)
            {
                IntPtr listBoxHandle = FindWindowEx(hWnd, IntPtr.Zero, "ListBox", null);

                if (listBoxHandle != IntPtr.Zero)
                {
                    StringBuilder sb = new StringBuilder(256);
                    SendMessage(listBoxHandle, LB_GETTEXT, 0, sb);

                    SendMessage(listBoxHandle, LB_ADDSTRING, 0, new StringBuilder(text));

                    sb.Clear();
                    SendMessage(listBoxHandle, LB_GETTEXT, 0, sb);
                }
            }
        }
    }
}
