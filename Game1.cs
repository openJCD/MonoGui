﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MonoGui.Scenes;
using MonoGui.Engine.GUI;
using MonoGui.Engine;
using MonoTween;
using MonoGui.Designer;
using MonoGui.Utils;

namespace MonoGui
{
    internal class Game1 : Game
    {
        //PRIVATE MEMBERS
        private GraphicsDeviceManager graphicsManager;

        private SpriteBatch UISpriteBatch;
        private SpriteBatch SpriteBatch;
        Background BG;

        private KeyboardState oldKeyboardState;
        private TextLabel debug;
        public static ContentManager UIContentManager; //manager for fonts and shit
        SceneManager SceneManager;        
        public static UIRoot screenRoot;
        public Game1()
        {
            Content.RootDirectory = "Content";
            graphicsManager = new GraphicsDeviceManager(this);
            UIContentManager = new ContentManager(Content.ServiceProvider);
            UIContentManager.RootDirectory = "Content/GUI/";
            base.IsFixedTimeStep = false;
            //graphicsManager.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            UIEventHandler.OnKeyPressed += OnKeyPressed;
        }

        protected override void Initialize()
        {
            // test for button click events
            UIEventHandler.OnButtonClick += Game1_HandleOnButtonClick;
            
            SceneManager = Core.InitWithScenes(UIContentManager, graphicsManager, Window);
            base.Initialize();
        }
        private void Game1_HandleOnButtonClick(object sender, OnButtonClickEventArgs e)
        {
            Button Sender = (Button)sender;
            Debug.WriteLine(Sender.DebugLabel + " was pressed with event type " + e.event_type);

            if (e.event_type == EventType.QuitGame)
                Exit();
        }
        protected override void LoadContent()
        {
            UISpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Core.LoadAll(SceneManager, "Content/GUI/Scenes/", "default.scene");
            Container c = new Container(SceneManager.activeSceneUIRoot, 10,10,100,100, AnchorType.TOPLEFT);
            Button b = new Button(c, "hello", 0, 0, AnchorType.CENTRE, EventType.None,"btn_hello");

            Background.Import("Content/Backgrounds/crosses.json", Content, out BG);
        }
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            
            KeyboardState newKeyboardState = Keyboard.GetState();

            //testing designer
            Profiler.Begin("update total", (float)gameTime.ElapsedGameTime.TotalSeconds);
            SceneManager.Update(gameTime);
            DesignerContext.Update();
            oldKeyboardState = newKeyboardState;
            BG.Animate(gameTime);

            Profiler.Begin("Background highlight flare", (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (SceneManager.activeSceneUIRoot.TopmostWindow != null)
            {
            }

            Profiler.End("Background highlight flare");
            TweenManager.TickAllTweens((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
            Profiler.End("update total");
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.Begin("game1 draw total", (float)gameTime.ElapsedGameTime.TotalSeconds);

            GraphicsDevice.Clear(Color.Black);
            
            Core.UpdateFPS(gameTime);

            SpriteBatch.Begin();
            BG.Draw(SpriteBatch, new Rectangle(new Point(), new Point(graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight)));
            SpriteBatch.End();

            UISpriteBatch.Begin(rasterizerState:new RasterizerState() { ScissorTestEnable = true });
            SceneManager.Draw(UISpriteBatch);
            //ns_test.Draw(UISpriteBatch);
            UISpriteBatch.End();
            
            base.Draw(gameTime);
            Profiler.End("game1 draw total");
        }
        bool _designerEnabled = false;
        public void OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.first_key_as_string == "F1")
            {
                if (!_designerEnabled)
                {
                    _designerEnabled = true;
                    DesignerContext.Enable();
                } else if (_designerEnabled)
                {
                    _designerEnabled = false;
                    DesignerContext.Disable();
                }
            } 
        }
    }
}