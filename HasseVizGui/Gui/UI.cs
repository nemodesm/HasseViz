using System.Collections.Generic;
using HasseVizGui;
using ImGuiNET;
using Mapper.Util;
using Microsoft.Xna.Framework;
using NativeFileDialogSharp;
using GameTime = Microsoft.Xna.Framework.GameTime;

namespace Mapper.Gui;

public static class UI
{
    public static ImGuiRenderer Renderer;

    static UI()
    {
        Renderer = new ImGuiRenderer(HasseVizGui.Mapper.Instance);
        Renderer.RebuildFontAtlas();
    }
    
    public static void Render(GameTime gameTime)
    {
        Renderer.BeforeLayout(gameTime);
        {
            MainMenu();
        }
        Renderer.AfterLayout();
    }

    public static void MenuItemFile()
    {
        if (ImGui.BeginMenu("File")) {

            if (ImGui.MenuItem("Load Text"))
            {
                var path = Dialog.FileOpen();

                if (path is not null && path.IsOk)
                {
                    GraphHandler.LoadGraphText(path.Path);
                }
            }

            if (ImGui.MenuItem("Load SemiBinary"))
            {
                var path = Dialog.FileOpen();

                if (path is not null && path.IsOk)
                {
                    GraphHandler.LoadGraphBinText(path.Path);
                }
            }
            
            if (ImGui.BeginMenu("Open Recent")) {
                if (AppData.RecentFiles.Count == 0)
                {
                    ImGui.Text("No recent files");
                }
                
                foreach (var recentFile in AppData.RecentFiles)
                {
                    if (ImGui.MenuItem(recentFile))
                    {
                        GraphHandler.LoadGraphGeneral(recentFile);
                    }
                }
                
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Simplify"))
            {
                GraphHandler.Graph!.Simplify();
            }

            if (ImGui.MenuItem("Save as..."))
            {
                var path = Dialog.FileSave();
                /*Nfd.SaveDialog(out var path, 
                    new Dictionary<string, string>()
                    {
                        { "Map Files", "*.mpr" } // MaPpeR
                    },
                    $"{Map.AreaName}.mpr",
                    "maps/");*/

                if (path is not null && path.IsOk)
                {
                    HasseVizGui.Mapper.Instance.SaveMap(path.Path);
                }
            }
            ImGui.Separator();

            if (ImGui.MenuItem("Quit"))
            {
                HasseVizGui.Mapper.Instance.Exit();
            }
            
            ImGui.EndMenu();
        }
    }
    
    public static void MenuItemHelp()
    {
        if (ImGui.BeginMenu("Help")) {
            if (ImGui.MenuItem("Help", "F1"))
            {
                // TODO
            }
            ImGui.Separator();
            if (ImGui.MenuItem("User Guide"))
            {
                // TODO
            }

            if (ImGui.MenuItem("About"))
            {
                // TODO
            }
            ImGui.EndMenu();
        }
    }

    public static void MainMenuBar()
    {
        if (ImGui.BeginMainMenuBar()) {
            MenuItemFile();
            MenuItemHelp();
                
            ImGui.EndMainMenuBar();
        }
    }

    public static void MainMenu()
    {
        MainMenuBar();
    }
}