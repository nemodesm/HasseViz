using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using HasseVizLib;
using ImGuiNET;
using Mapper.Gui;
using Mapper.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HasseVizGui;

public sealed class Mapper : Game
{
    private GraphicsDeviceManager _graphics;
    public static SpriteBatch SpriteBatch;
    private RenderTarget2D UITarget;
    public static Mapper Instance { get; private set; }

    public static int ScreenWidth => Instance._graphics.PreferredBackBufferWidth;
    public static int ScreenHeight => Instance._graphics.PreferredBackBufferHeight;

    public Mapper()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreparingDeviceSettings += (_, args) =>
        {
            args.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.Reach;
            args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Instance = this;
        Window.AllowUserResizing = true;
        Window.AllowAltF4 = true;
    }

    protected override void Initialize()
    {
        Window.ClientSizeChanged += HandleWindowResized;
        Window.FileDrop += HandleFileDrop;
        
        AppData.Load();
        
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        AppData.Save();
        
        base.OnExiting(sender, args);
    }

    [MemberNotNull(nameof(SpriteBatch))]
    protected override void LoadContent()
    {
        CreateRenderTargets();
        
        GraphHandler.Graph = Graph.Build(new[,]
        {
            { true, false, false },
            { false, true, false },
            { true, true, false }
        });
        
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        
        Util.Draw.Initialize(GraphicsDevice);
        Util.Draw.SpriteBatch = SpriteBatch;

        // TODO: use this.Content to load your game content here
        CreateRenderTargets();
    }

    [MemberNotNull(nameof(UITarget))]
    private void CreateRenderTargets()
    { 
        UITarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height, false,
            GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
    }

    private void HandleWindowResized(object? sender, EventArgs e)
    {
        Debug.Trace($"Client resized window \uf119 (new size: {Window.ClientBounds.Width}x{Window.ClientBounds.Height})");
        GraphHandler.HandleGraphChange();
        CreateRenderTargets();
    }

    private void HandleFileDrop(object? sender, FileDropEventArgs e)
    {
        switch (e.Files.Length)
        {
            case 0:
                Debug.LogError($"{nameof(HandleFileDrop)}: Zero files dropped");
                return;
            case > 1:
                Debug.Trace($"{nameof(HandleFileDrop)}: Multiple files dropped: {e.Files.Aggregate((a, b) => $"{a}, {b}")}, only first will be used");
                break;
        }

        var file = e.Files[0];
        
        if (Directory.Exists(file))
        {
            Debug.LogError($"{nameof(HandleFileDrop)}: Directory dropped: {file}");
            return;
        }

        if (!File.Exists(file))
        {
            Debug.LogError($"{nameof(HandleFileDrop)}: Unknown file dropped: {file}");
            return;
        }

        // TODO: Load file
        GraphHandler.LoadGraphGeneral(file);
        Debug.Trace($"Dropped file: {file}");
    }

    private static Color ClearColor = new Color(0x212121);
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        UI.Render(gameTime);

        if (GraphHandler.GraphChanged)
        {
            GraphicsDevice.SetRenderTarget(UITarget);
            GraphicsDevice.Clear(ClearColor);
            SpriteBatch.Begin();
            DrawWorld();
            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }
        
        SpriteBatch.Begin();
        SpriteBatch.Draw(UITarget, Vector2.Zero, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawWorld()
    {
        GraphHandler.DrawGraph(Window.ClientBounds.Width, Window.ClientBounds.Height);
    }

    public void SaveMap(string pathPath)
    {
        var stream = File.Create(pathPath);
        UITarget.SaveAsPng(stream, UITarget.Width, UITarget.Height);
    }
}