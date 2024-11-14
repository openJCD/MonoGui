using MonoGui.Engine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoGui.Engine.GUI
{
    public class UIRoot : IDisposable, Control
    {
        private static MouseState oldmousestate;
        private static MouseState newmousestate;

        private static List<TextInput> _textInputSelectLookup = new List<TextInput>();
        public WindowContainer TopmostWindow => _orderedChildren[0] as WindowContainer;
        public bool IsUnderMouseFocus => true;
        public static MouseState MouseState { get => oldmousestate; }

        private KeyboardState oldkstate;
        private KeyboardState newkstate;

        private int width, windowwidth, paddingx;

        private int height, windowheight, paddingy;
        private List<Container> base_containers;
        private GraphicsDeviceManager graphicsInfo;

        [LuaHide] public List<Container> ChildContainers => _children.Cast<Container>().ToList();


        // list of containers to use for click propagation / blocking (topmost panel is index 0)
        List<Control> _orderedChildren
        {
            get
            {
                List<Control> _oc = new List<Control>();
                _oc.AddRange(_children);
                _oc.Reverse();
                return _oc;
            }
        }

        [LuaHide]
        public List<Widget> ChildWidgets { get; set; }
        public string DebugLabel { get { return "UI Root"; } }
        public float LocalY { get; set; }
        public float Width { get { return width; } set => width = (int)value; }
        public float Height { get { return height; } set => height = (int)value; }
        public float XPos { get; set; }
        public float YPos { get; set; }
        public AnchorCoord Anchor { get; }

        public int PaddingX
        {
            get { return paddingx; }
            set
            {
                paddingx = value;
                XPos = value;
                Width = windowwidth - value * 2;
            }
        }
        public int PaddingY
        {
            get { return paddingy; }
            set
            {
                paddingy = value;
                YPos = value;
                Height = windowheight - value * 2;
            }
        }
        public float Alpha { get; set; } = 255f;

        [LuaHide]
        public Container draggedWindow { get; set; }

        public Vector2 MouseDelta;
        private readonly List<Control> _children = new List<Control>();

        public UIRoot()
        {
            graphicsInfo = Core.GraphicsManager;
            windowwidth = (int)(Width = Theme.DisplayWidth);
            windowheight = (int)(Height = Theme.DisplayHeight);
        }
        public UIRoot(GraphicsDeviceManager graphicsInfo)
        {
            Initialise(graphicsInfo);
            windowwidth = (int)(Width = Theme.DisplayWidth);
            windowheight = (int)(Height = Theme.DisplayHeight);
        }
        public void Initialise(GraphicsDeviceManager graphicsInfo)
        {
            this.graphicsInfo = graphicsInfo;
            Width = graphicsInfo.PreferredBackBufferWidth;
            Height = graphicsInfo.PreferredBackBufferHeight;
        }

        /// <summary>
        /// Destroy and clear all children
        /// </summary>
        [LuaHide]
        public void Dispose()
        {
            ChildContainers.ToList().ForEach(c => c.Dispose());
            new List<Container>();
            _textInputSelectLookup = new List<TextInput>();
        }
        [LuaHide]
        public void Update()
        {
            Mouse.SetCursor(MouseCursor.Arrow);
            draggedWindow = null;
            newkstate = Keyboard.GetState();
            newmousestate = Mouse.GetState();
            MouseDelta = newmousestate.Position.ToVector2() - oldmousestate.Position.ToVector2();
            foreach (var child in _children)
            {
                child.Update(oldmousestate, newmousestate);
            }
            GetHoveredContainers();
            if (newmousestate.RightButton == ButtonState.Pressed && oldmousestate.RightButton == ButtonState.Released)
            {
                UIEventHandler.onMouseClick(this, new MouseClickArgs { mouse_data = newmousestate });
                Click(newmousestate.Position.ToVector2(), ClickMode.Down, MouseButton.Right);
            }
            if (newmousestate.LeftButton == ButtonState.Pressed && oldmousestate.LeftButton == ButtonState.Released)
            {
                UIEventHandler.onMouseClick(this, new MouseClickArgs { mouse_data = newmousestate });
                Click(newmousestate.Position.ToVector2(), ClickMode.Down, MouseButton.Left);
            }
            if (newmousestate.LeftButton == ButtonState.Released && oldmousestate.LeftButton == ButtonState.Pressed)
            {
                UIEventHandler.onMouseUp(this, new MouseClickArgs { mouse_data = newmousestate });
                Click(newmousestate.Position.ToVector2(), ClickMode.Up, MouseButton.Left);
            }
            if (newkstate.GetPressedKeyCount() == 0 && oldkstate.GetPressedKeyCount() > 0)
            {
                UIEventHandler.onKeyReleased(this, new KeyReleasedEventArgs() { released_keys = oldkstate.GetPressedKeys() });
            }
            if (oldkstate.GetPressedKeyCount() < 1 && newkstate.GetPressedKeyCount() > 0)
            {
                UIEventHandler.onKeyPressed(this, new KeyPressedEventArgs() { pressed_keys = newkstate.GetPressedKeys() });
            }

            UIEventHandler.onUIUpdate(this, EventArgs.Empty);
            oldkstate = newkstate;
            oldmousestate = newmousestate;
            _movedToNext = false;
        }

        public List<Control> Children => _children;

        public Control Add(Control c)
        {
            _children.Add(c);
            return this;
        }

        public void Remove(Control c)
        {
            _children.Remove(c);
        }

        public void Draw(SpriteBatch guiSpriteBatch)
        {
            foreach (Control c in _children)
                c.Draw(guiSpriteBatch);
        }

        public void Update(MouseState oldState, MouseState newState) => Update();

        public float LocalX { get; set; }

        public void AddContainer(Container containerToAdd)
        {
            ChildContainers.Add(containerToAdd);
        }
        [LuaHide]
        public void PrintUITree()
        {
            Debug.WriteLine("Whole UI Tree is as follows:");
            foreach (Container container in ChildContainers)
            {
                container.PrintChildren(0);
            }
        }
        public void ApplyNewSettings()
        {
            Width = Theme.DisplayWidth;
            Height = Theme.DisplayHeight;
        }
        [LuaHide]
        public void BringWindowToTop(Container window)
        {
            if (ChildContainers.Remove(window))
                ChildContainers.Add(window);
            draggedWindow = window;
        }

        public void PushWindowToBottom(Container window)
        {
            ChildContainers.Remove(window);
            ChildContainers.Insert(0, window);
        }

        public List<Keys> GetPressedKeys()
        {
            return newkstate.GetPressedKeys().ToList();
        }

        public List<Container> GetHoveredContainers()
        {
            List<Container> returnc = new List<Container>();
            foreach (Container c in ChildContainers)
            {
                if (c.IsUnderMouseFocus)
                {
                    returnc.Add(c);
                    //c.DrawBorder = false;
                }
                //else c.DrawBorder = true;
            }
            return returnc;
        }

        public void OnWindowResize(GameWindow w)
        {
            windowwidth = w.ClientBounds.Width;
            windowheight = w.ClientBounds.Height;
            Height = w.ClientBounds.Height - PaddingY * 2;
            Width = w.ClientBounds.Width - PaddingX * 2;
            ChildContainers.ForEach(cont => cont.ResetPosition());
        }

        public void Click(Vector2 mousePos, ClickMode cmode, MouseButton buttonType)
        {
            // should fetch the topmost contained clicked and skip the other ones
            foreach (Control c in _orderedChildren)
            {
                if (c.IsUnderMouseFocus)
                {
                    c.Click(mousePos, cmode, MouseButton.Left);
                    return;
                }
            }
        }

        public UIRoot FindRoot()
        {
            return this;
        }

        public void RemoveChildWidget(Widget w)
        {
            ChildWidgets.Remove(w);
        }

        static bool _movedToNext = false;
        internal static void MoveNextTextFieldFrom(TextInput txt)
        {
            if (_movedToNext) return;
            int index = _textInputSelectLookup.IndexOf(txt);
            txt.SetInactive();
            index++;
            if (_textInputSelectLookup.Count - 1 >= index)
            {
                var tgt = _textInputSelectLookup[index];
                if (tgt.Enabled && !tgt.Active)
                {
                    _textInputSelectLookup.ToList().ForEach(tx => tx.SetInactive());
                    tgt.SetActive();
                    _movedToNext = true;
                }
            }
        }
        internal static void RegisterTextField(TextInput txt)
        {
            _textInputSelectLookup.Add(txt);
        }
    }
    public enum ClickMode
    {
        Down,
        Up
    }

    public enum MouseButton
    {
        Left, 
        Middle, 
        Right
    }
}
