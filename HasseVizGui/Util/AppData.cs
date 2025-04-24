using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HasseVizLib;
using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace HasseVizGui.Util;

public static class AppData
{
    private const int MaxRecentFiles = 10;
    private const string Version = "1";
    public static Vector3 BGCol = new(0x21/255f, 0x21/255f, 0x21/255f);
    public static Vector3 VertCol = new(1);
    private const string FileName = "#graphvis#";
    public static List<string> RecentFiles = new();
    private static string AppDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/HasseViz/";
    private static string FilePath = Path.Combine(AppDataPath, FileName);
    public static Color BGColMG => new Color(BGCol);
    public static Color VertColMG => new Color(VertCol);

    public static void AddRecentFile(string path)
    {
        RecentFiles.Remove(path);
        RecentFiles.Insert(0, path);

        if (RecentFiles.Count > MaxRecentFiles)
        {
            RecentFiles.RemoveRange(MaxRecentFiles, RecentFiles.Count - MaxRecentFiles);
        }
    }

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            var lines = File.ReadAllLines(FilePath);
            if (lines[0] != Version)
            {
                Debug.LogWarning($"Version mismatch, aborting load (expected {Version}, got {lines[0]})");
                return;
            }

            //BGCol = Vector3.Parse(lines[1]);
            //VertCol = Vector3.Parse(lines[2]);
            RecentFiles = new List<string>(lines.Skip(3));
        }
    }
    
    public static void Save()
    {
        using (var f = File.CreateText(FilePath))
        {
            f.WriteLine(Version);
            f.WriteLine(BGCol.ToString());
            f.WriteLine(VertCol.ToString());
            foreach (var recentFile in RecentFiles)
            {
                f.WriteLine(recentFile);
            }
        }
    }
}