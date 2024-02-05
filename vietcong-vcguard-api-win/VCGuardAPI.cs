using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using vietcong_vcguard_api_win;

/// <summary>
/// VCGuard API is api for sending, listening and some settings and other..
/// </summary>
public class VCGuardAPI
{
    private const int WM_SETTEXT = 0x000C;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int VK_RETURN = 0x0D;
    private const int BM_CLICK = 0x00F5;
    private const int WM_GETTEXT = 0x000D;

    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);
    [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)] private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder lParam);

    private static readonly ListBoxSetData lbsd = new ListBoxSetData();
    private static readonly ListBoxGetData lbgd = new ListBoxGetData();

    private static bool connected = false;
    private static bool logging = true;

    private static IntPtr hwdHandle;
    private static IntPtr cbtnHandle;
    private static IntPtr txtHandle;
    private static string title;

    /// <summary>
    /// Author of the assembly file.
    /// </summary>
    public static string API_AUTHOR = "Floxen; ten.neznamey@seznam.cz; v2.1.5";

    /// <summary>
    /// Security settings.
    /// </summary>
    private static int waitBeforeAct = 1;
    private static int maxTexLength = 2048;

    /// <summary>
    /// Security levels.
    /// LOW
    /// </summary>
    public static readonly int NO_SECURITY = 0;
    /// <summary>
    /// Normal security
    /// </summary>
    public static readonly int NORMAL_SECURITY = 1;
    /// <summary>
    /// High secuity
    /// </summary>
    public static readonly int HIGH_SECURITY = 2;

    /// <summary>
    /// Connect to the server using the port.
    /// </summary>
    /// <param name="port"></param>
    public static void ConnectTo(string port)
    {
        if (logging) Console.Write("[API] Connecting to server..");

        foreach (var process in Process.GetProcesses())
        {
            if (process.MainWindowTitle.EndsWith(port))
            {
                hwdHandle = process.MainWindowHandle;
                title = process.MainWindowTitle.Trim();
                connected = true;
                if (logging) Console.WriteLine(" Connected!");
                new VCPath(GetWorkingDirectory());
                break;
            }
        }

        if (connected)
        {
            cbtnHandle = FindWindowEx(hwdHandle, IntPtr.Zero, "Button", null);
            txtHandle = FindWindowEx(hwdHandle, IntPtr.Zero, "Edit", null);
        }
        else
        {
            if (logging)
            {
                ErrorSignal(" Failed!");
                ErrorSignal("[API] Make sure you are using vcguard tool. You can download here: https://www.moddb.com/downloads/vcguard-12-1-beta");
            }
        }
    }

    /// <summary>
    /// Get handle of main server window.
    /// </summary>
    /// <returns></returns>
    public static IntPtr GetWindowHandle()
    {
        return hwdHandle;
    }

    /// <summary>
    /// Get title of main server gui window.
    /// </summary>
    /// <returns></returns>
    public static string GetWindowTitle()
    {
        return title;
    }

    /// <summary>
    /// Get PID of server process.
    /// </summary>
    /// <returns></returns>
    public static int GetPID()
    {
        foreach (var proc in Process.GetProcesses())
        {
            if (proc.MainWindowHandle == hwdHandle)
            {
                return proc.Id;
            }
        }

        return -1;
    }

    /// <summary>
    /// Disconnect from the server.
    /// </summary>
    public static void Disconnect()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return;
        }

        hwdHandle = IntPtr.Zero;
        title = null;
        connected = false;
    }

    /// <summary>
    /// Check if is the server connected with the API.
    /// </summary>
    /// <returns></returns>
    public static bool IsConnected()
    {
        return connected;
    }

    /// <summary>
    /// Sets the security level of the API (good when u make public server tool)
    /// </summary>
    /// <param name="lvl"></param>
    public static void SetSecurityLevel(int lvl)
    {
        if (logging) Console.Write("[API] Security level was set to: ");

        if (lvl == 0)
        {
            maxTexLength = 2048;
            waitBeforeAct = 1;

            if (logging) Console.WriteLine("None");

            return;
        }

        else if (lvl == 1)
        {
            maxTexLength = 512;
            waitBeforeAct = 50;

            if (logging) Console.WriteLine("Normal");

            return;
        }

        else if (lvl == 2)
        {
            maxTexLength = 256;
            waitBeforeAct = 200;

            if (logging) Console.WriteLine("High");

            return;
        }

        if (logging) Console.WriteLine("None");
        maxTexLength = 2048;
        waitBeforeAct = 2;
    }

    /// <summary>
    /// Enable or disable logging debug into your console application.
    /// </summary>
    /// <param name="enabled"></param>
    public static void SetLogging(bool enabled)
    {
        logging = enabled;

        if (logging) Console.WriteLine("[API] Logging changed to " + enabled);
    }

    /// <summary>
    /// Sends dc message from server.
    /// </summary>
    /// <param name="text"></param>
    public static void Say(string text)
    {
        SendToGame("say \"" + Translate(text).Trim() + "\"");
    }

    /// <summary>
    /// Sends admin message to server.
    /// </summary>
    /// <param name="text"></param>
    public static void Adminsay(string text)
    {
        SendToGame("adminsay \"" + Translate(text).Trim() + "\"");
    }

    /// <summary>
    /// Sends server message to server.
    /// </summary>
    /// <param name="text"></param>
    public static void Serversay(string text)
    {
        if (GetVersion() == "12.1" || GetVersion() == "11.60")
        {
            SendToGame(".serversay " + text);
        }
        else
        {
            if (logging) Console.WriteLine("[API] Unsupported console command in VCGuard version " + GetVersion());
        }
    }

    /// <summary>
    /// Sends red text to game.
    /// </summary>
    /// <param name="text"></param>
    public static void Redsay(string text)
    {
        if (GetVersion() == "12.1" || GetVersion() == "11.60")
        {
            SendToGame(".redsay " + text);
        }
        else
        {
            if (logging) Console.WriteLine("[API] Unsupported console command in VCGuard version " + GetVersion());
        }
    }

    /// <summary>
    /// Sends the raw string to vcguard (this can be hazardous when u making public tool and uses this API)
    /// </summary>
    /// <param name="data"></param>
    public static void RawSend(string data)
    {
        SendToGame(data);
    }

    /// <summary>
    /// Clears the console view on vcguard window.
    /// </summary>
    public static void ClearList()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return;
        }

        if (hwdHandle != IntPtr.Zero && cbtnHandle != IntPtr.Zero)
        {
            StringBuilder buttonText = new StringBuilder(256);

            GetWindowText(cbtnHandle, buttonText, buttonText.Capacity);

            if (buttonText.ToString() == "Clear List")
            {
                SendMessage(cbtnHandle, BM_CLICK, IntPtr.Zero, null);
            }
        }
    }

    /// <summary>
    /// Get current vcguard version.
    /// </summary>
    /// <returns></returns>
    public static string GetVersion()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return "0.0";
        }

        return title.Split('v')[1].Split(' ')[0].Trim();
    }

    /// <summary>
    /// Get full mission (map) name.
    /// </summary>
    /// <returns></returns>
    public static string GetMissionName()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return null;
        }

        try
        {
            return ExtractTextFromControls().Split(new[] { "Mission:" }, StringSplitOptions.None)[1].Split('\n')[0].Trim();
        }
        catch { return null; }
    }

    /// <summary>
    /// Get online players count.
    /// </summary>
    /// <returns></returns>
    public static int GetPlayerCount()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return -1;
        }

        try
        {
            return int.Parse(ExtractTextFromControls().Split(new[] { "Players:" }, StringSplitOptions.None)[1].Split('/')[0].Trim());
        }
        catch { return -1; }
    }

    /// <summary>
    /// Parses last written message tries from /dev/chatlog.txt
    /// </summary>
    /// <returns></returns>
    public static string[] GetLastWrittenMessage()
    {
        try
        {
            if (File.Exists(VCPath.CHATLOG))
            {
                string[] oArr = new string[3];
                string chat = File.ReadLines(VCPath.CHATLOG).Last();

                string id = chat.Substring(24, chat.Length - 24).Split('[')[1].Split(']')[0].Trim();
                string name = chat.Split(new[] { "[" + id + "] " }, StringSplitOptions.None)[1].Split(':')[0].Trim();
                string content = chat.Split(new[] { name + " :" }, StringSplitOptions.None)[1].Trim();

                oArr[0] = id;
                oArr[1] = name;
                oArr[2] = content;

                return oArr;
            }
            else
            {
                return null;
            }
        }
        catch { return null; }
    }

    /// <summary>
    /// Parse /logs/connections.txt log file tries.
    /// </summary>
    /// <returns></returns>
    public static string[] GetLastConnectionInfo()
    {
        try
        {
            if (File.Exists(VCPath.LOGGING))
            {
                string[] oArr = new string[4];
                string[] arr = GetLastFileLine(VCPath.LOGGING).Split('/');

                oArr[0] = arr[0].Substring(26, arr[0].Length - 26).Trim();
                oArr[1] = arr[1].Trim();
                oArr[2] = arr[2].Trim();
                oArr[3] = arr[3].Trim();

                return oArr;
            }
            else
            {
                return null;
            }
        }
        catch { return null; }
    }

    /// <summary>
    /// Get file size (LONG).
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static long GetFilesize(string filePath)
    {
        try
        {
            if (!IsConnected())
            {
                if (logging) ErrorSignal("[API] Server is not connected!");
                return -1;
            }

            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }

            return -1;
        }
        catch { return -1; }
    }


    /// <summary>
    /// Get a last file line string.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetLastFileLine(string filePath)
    {
        try
        {
            if (!IsConnected())
            {
                if (logging) ErrorSignal("[API] Server is not connected!");
                return null;
            }

            if (File.Exists(filePath))
            {
                return File.ReadLines(filePath).Last();
            }
            else
            {
                return null;
            }
        }
        catch { return null; }
    }

    /// <summary>
    /// Get directory of where was server started.
    /// </summary>
    /// <returns></returns>
    public static string GetWorkingDirectory()
    {
        try
        {
            if (!IsConnected())
            {
                if (logging) ErrorSignal("[API] Server is not connected!");
                return "";
            }

            foreach (var proc in Process.GetProcesses())
            {
                if (proc.MainWindowHandle == hwdHandle)
                {
                    return proc.MainModule.FileName.Replace("\\" + proc.MainModule.ModuleName, "").Trim();
                }
            }

            return null;
        }
        catch { return null; }
    }

    /// <summary>
    /// Gets a data from vcguard listbox.
    /// </summary>
    /// <returns></returns>
    public static string GetListboxData()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return null;
        }

        return lbgd.GetData(hwdHandle);
    }

    private static void ErrorSignal(string s)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(s);
        Console.ResetColor();
    }

    private static string GetTextFromWindow(IntPtr windowHandle)
    {
        try
        {
            int length = GetWindowTextLength(windowHandle);

            if (length > 0)
            {
                StringBuilder sb = new StringBuilder(length + 1);
                _ = SendMessage(windowHandle, WM_GETTEXT, length + 1, sb);
                return sb.ToString();
            }

            return null;
        }
        catch { return null; }
    }

    private static string ExtractTextFromControls()
    {
        try
        {
            string data = "";

            IntPtr childHandle = IntPtr.Zero;

            while ((childHandle = FindWindowEx(hwdHandle, childHandle, null, null)) != IntPtr.Zero)
            {
                string text = GetTextFromWindow(childHandle);

                if (!string.IsNullOrEmpty(text))
                {
                    data += text.Trim() + "\n";
                }
            }

            return data;
        }
        catch { return null; }
    }

    private static string Translate(string text)
    {
        text = text.Replace("\"", "''");
        return text;
    }

    private static void SendToGame(string data)
    {
        try
        {
            if (!IsConnected())
            {
                if (logging) ErrorSignal("[API] Server is not connected!");
                return;
            }

            if (data.ToLower().Contains("devexcp"))
            {
                if (waitBeforeAct >= 200)
                {
                    if (logging) ErrorSignal("[API] Command 'devexcp' is unsafe in this context. If you want use it, set security level to NO_SECURITY or NORMAL_SECURITY.\n");
                    return;
                }
            }

            Thread.Sleep(waitBeforeAct);
            if (data.Length > maxTexLength) return;

            if (logging) Console.WriteLine("[API] Sending advertisement: " + data.Trim());

            SendMessage(txtHandle, WM_SETTEXT, IntPtr.Zero, data);
            SendMessage(txtHandle, 0x0201, IntPtr.Zero, null);
            SendMessage(txtHandle, 0x0202, IntPtr.Zero, null);
            PostMessage(txtHandle, WM_KEYDOWN, (IntPtr)VK_RETURN, IntPtr.Zero);
            PostMessage(txtHandle, WM_KEYUP, (IntPtr)VK_RETURN, IntPtr.Zero);
        }
        catch { }
    }

    /// <summary>
    /// Struct of player List.
    /// </summary>
    public class PlayerStruct
    {
        /// <summary>
        /// Player ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Player PING
        /// </summary>
        public int Ping { get; set; }
        /// <summary>
        /// Player NAME
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Player KILLS
        /// </summary>
        public int Kills { get; set; }
        /// <summary>
        /// Player DEATHS
        /// </summary>
        public int Deaths { get; set; }
    }

    private static readonly List<PlayerStruct> playerList = new List<PlayerStruct>();

    /// <summary>
    /// Get complete player list including player id, name, kills, deaths and ping.
    /// </summary>
    /// <returns></returns>
    public static List<PlayerStruct> GetPlayerList()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return null;
        }

        string dataBefore = lbgd.GetData(hwdHandle).Trim();
        ClearList();
        Thread.Sleep(20);
        RawSend("LIST");
        Thread.Sleep(20);
        string getData = lbgd.GetData(hwdHandle).Trim();
        ClearList();

        playerList.Clear();

        foreach (var players in getData.Split('\n'))
        {
            if (players.Contains("kills: ") && players.Contains("deaths: ") && players.Contains("ping: "))
            {
                string raw = players.Substring(10).Trim();

                string str_id = raw.Split('[')[1].Split(']')[0].Trim();
                string str_name = raw.Split(new[] { "[" + str_id + "] " }, StringSplitOptions.None)[1].Split(new[] { "kills: " }, StringSplitOptions.None)[0].Trim();
                string str_kills = raw.Split(new[] { "kills: " }, StringSplitOptions.None)[1].Split(new[] { "deaths: " }, StringSplitOptions.None)[0].Trim();
                string str_deaths = raw.Split(new[] { "deaths: " }, StringSplitOptions.None)[1].Split(new[] { "ping: " }, StringSplitOptions.None)[0].Trim();
                string str_ping = raw.Split(new[] { "ping: " }, StringSplitOptions.None)[1].Trim();

                int id = int.Parse(str_id);
                int kills = int.Parse(str_kills);
                int deaths = int.Parse(str_deaths);
                int ping = int.Parse(str_ping);

                playerList.Add(new PlayerStruct { Id = id, Name = str_name, Kills = kills, Deaths = deaths, Ping = ping });
            }
        }

        foreach (var dataLine in dataBefore.Split('\n'))
        {
            lbsd.SetData(hwdHandle, dataLine.Trim());
        }

        return playerList;
    }

    /// <summary>
    /// Get custom command response.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string GetCommandResponse(string command)
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return null;
        }

        string dataBefore = lbgd.GetData(hwdHandle).Trim();
        ClearList();
        Thread.Sleep(20);
        RawSend(command);
        Thread.Sleep(20);
        string getData = lbgd.GetData(hwdHandle).Trim();
        ClearList();

        foreach (var dataLine in dataBefore.Split('\n'))
        {
            lbsd.SetData(hwdHandle, dataLine.Trim());
        }

        return getData;
    }

    /// <summary>
    /// Get live server FPS.
    /// </summary>
    /// <returns></returns>
    public static int GetServerFPS()
    {
        if (!IsConnected())
        {
            if (logging) ErrorSignal("[API] Server is not connected!");
            return -1;
        }

        string extracted = ExtractTextFromControls();

        string fps = "-1";

        if (extracted.Contains("Server FPS:"))
        {
            fps = extracted.Split(new[] { "Server FPS:" }, StringSplitOptions.None)[1].Split('\n')[0].Trim();

            if (fps.Contains("(idle)"))
            {
                fps = fps.Split(new[] { "(idle)" }, StringSplitOptions.None)[0].Trim();
            }
        }

        return int.Parse(fps);
    }
}
