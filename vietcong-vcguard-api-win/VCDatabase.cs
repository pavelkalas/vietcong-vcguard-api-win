using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// VCDatabase is simple database for storing any data.
/// </summary>
public class VCDatabase
{
    private static readonly string dir = Environment.CurrentDirectory + "\\tmpdb\\";

    /// <summary>
    /// Create the database and store data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void Save(string name, string value)
    {
        name = Md5(name);

        name = dir + name;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (!File.Exists(name))
        {
            File.WriteAllText(name, value);
        }
    }

    /// <summary>
    /// Delete the database record.
    /// </summary>
    /// <param name="name"></param>
    public static void Delete(string name)
    {
        name = Md5(name);

        name = dir + name;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (File.Exists(name))
        {
            File.Delete(name);
        }
    }

    /// <summary>
    /// Get data from database.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Get(string name)
    {
        name = Md5(name);

        name = dir + name;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (File.Exists(name))
        {
            return File.ReadAllText(name);
        }

        return null;
    }

    /// <summary>
    /// Converting the text to safe database folder structure name.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string Md5(string input)
    {
        string result;

        using (MD5 hash = MD5.Create())
        {
            result = String.Join("", from ba in hash.ComputeHash(Encoding.UTF8.GetBytes(input)) select ba.ToString("x2"));
        }

        return result;
    }
}
