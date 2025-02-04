﻿using MonoGui.Engine.GUI;
using MonoGui.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.CodeDom;
using System.IO;


namespace MonoGui.Engine
{
    public static class Core
    {
        public static double FPS = 0.0;

        static object syncRoot = new object();
        static GraphicsDeviceManager _graphicsManager;
        public static GraphicsDeviceManager GraphicsManager
        {
            get
            {

                return _graphicsManager;
            }
            set
            {
                _graphicsManager = value;
            }
        }
        public static GameWindow Window;

        public static ContentManager Content { get; private set; }
        static SceneManager sm;
        static FileSystemWatcher _scenesWatcher = new FileSystemWatcher("Content/GUI/Scenes/");

        static string _settingsPath;
        public static SceneManager InitWithScenes(ContentManager c, GraphicsDeviceManager g, GameWindow w, string settingsPath = "Content/Saves/settings.ini")
        {
            GraphicsManager = g;
            Window = w;
            Content = c;
            Window.AllowUserResizing = true;
            Window.KeyDown += OnKeyDown;
            Window.ClientSizeChanged += OnResize;
            _settingsPath = settingsPath;
            sm = new SceneManager(c, g);
            Theme.LoadIniFile(settingsPath, Content);
            SetScreenBounds(Theme.DisplayWidth, Theme.DisplayHeight);
            return sm;
        }

        public static UIRoot InitEssential(GraphicsDeviceManager g, GameWindow w, ContentManager c, string settingsPath = "Content/Saves/settings.ini")
        {
            GraphicsManager = g;
            Window = w;
            Content = c;
            Window.AllowUserResizing = true;
            Window.KeyDown += OnKeyDown;
            Window.ClientSizeChanged += OnResize;
            Theme.LoadIniFile(settingsPath, Content);
            SetScreenBounds(Theme.DisplayWidth, Theme.DisplayHeight);

            return new UIRoot();
        }
        
        /// <summary>
        /// Load everything, including scenes from sceneFolder using the given SceneManager.
        /// Essentially the MonoGui Core with Scenes enabled.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="sceneFolder"></param>
        /// <param name="sceneEntryPoint"></param>
        public static void LoadAll(SceneManager s, string sceneFolder, string sceneEntryPoint)
        {
            s.CreateScenesFromFolder(sceneFolder);
            s.LoadScene(sceneEntryPoint);
        }
        
        public static double GetFPS(GameTime gt)
        {
            return (1 / gt.ElapsedGameTime.TotalSeconds);
        }
        public static void UpdateFPS(GameTime gt)
        {
            FPS = GetFPS(gt);
        }
        public static void OnResize(object sender, EventArgs e)
        {
            SetScreenBounds(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }
        public static void ReloadAt(SceneManager manager, string scene)
        {
            // global event called so application can hot-reload itself
            UIEventHandler.onHotReload(manager, new HotReloadEventArgs() { graphicsDeviceReference = GraphicsManager });
            Theme.Unload();
            Theme.LoadIniFile(_settingsPath, Content);
            SetScreenBounds(Theme.DisplayWidth, Theme.DisplayHeight);

            manager.LoadScene(scene);
        }
        static void SetScreenBounds(int w, int h)
        {
            GraphicsManager.PreferredBackBufferWidth = w;
            GraphicsManager.PreferredBackBufferHeight = h;
            GraphicsManager.ApplyChanges();
        }
        public static void UpdateWatch(object sender, FileSystemEventArgs e)
        {
            UIEventHandler.sendDebugMessage(sender, "File changes detected - press F5 to hot reload.");
        }
        public static void OnKeyDown(object sender, InputKeyEventArgs e)
        {
            if (e.Key == Keys.F5)
                ReloadAt(sm, SceneManager.ActiveScene?.Name);
        }
    }
}
