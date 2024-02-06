using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;

/// <summary>
/// All generics path for game and vcguard.
/// </summary>
public class VCPath
{
    /// <summary>
    /// Chatlog file.
    /// </summary>
    public static string CHATLOG = "/dev/chatlog.txt";

    /// <summary>
    /// vcguard connections file.
    /// </summary>
    public static string LOGGING = "/logs/connections.txt";

    /// <summary>
    /// Console file.
    /// </summary>
    public static string CONSOLE = "/logs/console.txt";

    /// <summary>
    /// /logs/ directory.
    /// </summary>
    public static string LOGS_DIR = "/logs";

    /// <summary>
    /// /dev/ directory.
    /// </summary>
    public static string DEV_DIR = "/dev";

    /// <summary>
    /// Root directory (running server instance)
    /// </summary>
    public static string SERVER_ROOT_FOLDER = Environment.CurrentDirectory;

    /// <summary>
    /// Constructor for build these path structure.
    /// </summary>
    /// <param name="path"></param>
    public VCPath(string path)
    {
        CHATLOG = path + "\\dev\\chatlog.txt";
        LOGGING = path + "\\logs\\connections.txt";
        CONSOLE = path + "\\logs\\console.txt";
        LOGS_DIR = path + "\\logs";
        DEV_DIR = path + "\\dev";
        SERVER_ROOT_FOLDER = path;
    }
}
