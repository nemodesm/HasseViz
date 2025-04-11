using System;
using System.Collections.Generic;
using System.IO;

namespace Mapper.Util;

public static class AppData
{
    private const string FileName = "#graphvis#";
    public static List<string> RecentFiles = new();

    public static void AddRecentFile(string path)
    {
        RecentFiles.Remove(path);
        RecentFiles.Insert(0, path);
    }

    public static void Load()
    {
        if (File.Exists(FileName))
        {
            var lines = File.ReadAllLines(FileName);
            RecentFiles.AddRange(lines);
        }
    }
    
    public static void Save()
    {
        using (var f = File.CreateText(FileName))
        {
            foreach (var recentFile in RecentFiles)
            {
                f.WriteLine(recentFile);
            }
        }
    }
}