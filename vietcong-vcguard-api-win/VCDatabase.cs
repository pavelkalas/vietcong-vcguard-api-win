using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// VCDatabase is simple database for storing any data.
/// </summary>
public class VCDatabase
{
    private static readonly string rootDirectory = Environment.CurrentDirectory + "\\db";
    private static string dir = null;

    /// <summary>
    /// VCDatabase constructor.
    /// </summary>
    public VCDatabase()
    {
        if (Directory.Exists(rootDirectory))
        {
            Directory.CreateDirectory(rootDirectory);
        }
    }

    /// <summary>
    /// Create the database and store data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static void RecordCreate(string name, string value)
    {
        if (dir == null)
        {
            Console.WriteLine("[VCAPI] Please, select database!");
            return;
        }

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
    /// Checks if exists
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool RecordExists(string name)
    {
        if (dir == null)
        {
            Console.WriteLine("[VCAPI] Please, select database!");
            return false;
        }

        name = Md5(name);
        name = dir + name;

        return File.Exists(name);
    }

    /// <summary>
    /// Delete the database record.
    /// </summary>
    /// <param name="name"></param>
    public static void RecordDelete(string name)
    {
        if (dir == null)
        {
            Console.WriteLine("[VCAPI] Please, select database!");
            return;
        }

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
    public static string RecordRead(string name)
    {
        if (dir == null)
        {
            Console.WriteLine("[VCAPI] Please, select database!");
            return null;
        }

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
    /// Creates a database.
    /// </summary>
    /// <param name="name"></param>
    public static void DatabaseCreate(string name)
    {
        name = Md5(name);

        if (!Directory.Exists(rootDirectory + "\\" + name))
        {
            Directory.CreateDirectory(rootDirectory + "\\" + name);
        }

        dir = rootDirectory + "\\" + name;
    }

    /// <summary>
    /// Delete the database with all records.
    /// </summary>
    /// <param name="name"></param>
    public static void DatabaseDrop(string name)
    {
        name = Md5(name);

        if (Directory.Exists(rootDirectory + "\\" + name))
        {
            Directory.Delete(rootDirectory + "\\" + name, true);
        }

        dir = null;
    }

    /// <summary>
    /// Checks if database exist.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool DatabaseExists(string name)
    {
        name = Md5(name);

        return Directory.Exists(rootDirectory + "\\" + name);
    }

    /// <summary>
    /// Select database to manipulate with records.
    /// </summary>
    /// <param name="name"></param>
    public static void DatabaseSelect(string name)
    {
        name = Md5(name);

        if (Directory.Exists(rootDirectory + "\\" + name))
        {
            dir = rootDirectory + "\\" + name;
        }
    }

    /// <summary>
    /// Database structure
    /// </summary>
    public class DatabaseStructure
    {
        /// <summary>
        /// Database name
        /// </summary>
        public string NAME;

        /// <summary>
        /// Records count in database.
        /// </summary>
        public int RECORDS_COUNT;
    }

    /// <summary>
    /// Get databases.
    /// </summary>
    /// <returns></returns>
    public static List<DatabaseStructure> GetDatabases()
    {
        List<DatabaseStructure> databases = new List<DatabaseStructure>();

        foreach (var database in Directory.GetDirectories(rootDirectory))
        {
            int records = 0;
            foreach (var record in Directory.GetFiles(database))
            {
                records++;
            }

            databases.Add(new DatabaseStructure { NAME = database, RECORDS_COUNT = records });
        }

        return databases;
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
