using System;
using System.Runtime.InteropServices;
using System.Text;

namespace vietcong_vcguard_api_win
{
    internal class ListBoxGetData
    {
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern int GetWindowTextLength(IntPtr hWnd);

        private readonly int LB_GETTEXT = 0x0189;
        private readonly int LB_GETTEXTLEN = 0x018A;
        private readonly int LB_GETCOUNT = 0x018B;

        /// <summary>
        /// Get vcguard listbox (console) data.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public string GetData(IntPtr hwnd)
        {
            IntPtr listBoxHandle = FindWindowEx(hwnd, IntPtr.Zero, "ListBox", null);

            string text = "";

            if (listBoxHandle != IntPtr.Zero)
            {
                int count = (int)SendMessage(listBoxHandle, LB_GETCOUNT, IntPtr.Zero, null);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        int textLength = SendMessage(listBoxHandle, LB_GETTEXTLEN, (IntPtr)i, null).ToInt32();

                        StringBuilder sb = new StringBuilder(textLength + 1);
                        SendMessage(listBoxHandle, LB_GETTEXT, (IntPtr)i, sb);
                        string itemText = sb.ToString();
                        text += itemText.Trim() + "\n";
                    }
                }
            }

            return text;
        }
    }
}
