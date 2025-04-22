using System;
using HasseVizGui;
using HasseVizLib;
using ImGuiNET;
using Mapper.Util;
using NativeFileDialogSharp;
using GameTime = Microsoft.Xna.Framework.GameTime;

namespace Mapper.Gui;

public static class UI
{
    public static ImGuiRenderer Renderer;

    private static bool CyclePopup;

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
            Popups();
        }
        Renderer.AfterLayout();
    }

    public static void Popups()
    {
        if (CyclePopup)
        {
            ImGui.OpenPopup(nameof(CyclePopup));
            CyclePopup = false;
        }

        if (ImGui.BeginPopup(nameof(CyclePopup)))
        {
            ImGui.Text("A cycle has been detected in you graph");
            ImGui.Text("Load aborted");
            ImGui.EndPopup();
        }
    }

    public static void MenuItemFile()
    {
        if (ImGui.BeginMenu("File")) {

            if (ImGui.MenuItem("Load"))
            {
                var path = Dialog.FileOpen();

                if (path is not null && path.IsOk)
                {
                    try
                    {
                        GraphHandler.LoadGraphGeneral(path.Path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message, e.StackTrace);
                        if (e.Message == "Cycle detected")
                        {
                            CyclePopup = true;
                        }
                    }
                }
            }

            /*if (ImGui.MenuItem("Load SemiBinary"))
            {
                var path = Dialog.FileOpen();

                if (path is not null && path.IsOk)
                {
                    try
                    {
                        GraphHandler.LoadGraphBinText(path.Path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message, e.StackTrace);
                        if (e.Message == "Cycle detected")
                        {
                            CyclePopup = true;
                        }
                    }
                }
            }
            */
            
            if (ImGui.BeginMenu("Open Recent")) {
                if (AppData.RecentFiles.Count == 0)
                {
                    ImGui.Text("No recent files");
                }
                
                foreach (var recentFile in AppData.RecentFiles.ToArray())
                {
                    if (ImGui.MenuItem(recentFile))
                    {
                        try
                        {
                            GraphHandler.LoadGraphGeneral(recentFile);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message, e.StackTrace);
                            if (e.Message == "Cycle detected")
                            {
                                CyclePopup = true;
                            }
                        }
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