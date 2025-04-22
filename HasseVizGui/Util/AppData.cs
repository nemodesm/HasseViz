using System;
using System.Collections.Generic;
using System.IO;

namespace Mapper.Util;

public static class AppData
{
    private const string FileName = "#graphvis#";
    public static List<string> RecentFiles = new();
    private static string AppDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/HasseViz/";
    private static string FilePath = Path.Combine(AppDataPath, FileName);

    public static void AddRecentFile(string path)
    {
        RecentFiles.Remove(path);
        RecentFiles.Insert(0, path);
    }

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            var lines = File.ReadAllLines(FilePath);
            RecentFiles.AddRange(lines);
        }
    }
    
    public static void Save()
    {
        using (var f = File.CreateText(FilePath))
        {
            foreach (var recentFile in RecentFiles)
            {
                f.WriteLine(recentFile);
            }
        }
    }
}