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
    /// Constructor for build these path structure.
    /// </summary>
    /// <param name="path"></param>
    public VCPath(string path)
    {
        CHATLOG = path + "\\dev\\chatlog.txt";
        LOGGING = path + "\\logs\\connections.txt";
        CONSOLE = path + "\\logs\\console.txt";
    }
}
