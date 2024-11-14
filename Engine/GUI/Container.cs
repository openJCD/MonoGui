using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Xml.Serialization;

namespace MonoGui.Engine.GUI
{
    public class Container : IDisposable, Control
    {
        private string _debugLabel;
        protected Control parent;
        protected Rectangle _boundingRectangle;
        protected AnchorCoord anchor;
        private bool isUnderMouseFocus;
        RasterizerState rstate;
        private List<Control> _children = new List<Control>();

        #region Properties/Members

        public bool BlockMouseClick = true;

        public LocalThemeProperties Theme = new LocalThemeProperties();
        public float Alpha { get; set; } = 255f;
        public bool IsUnderMouseFocus => isUnderMouseFocus;

        public virtual Control Parent
        {
            get => parent;
            protected set => parent = value;
        }

        public List<Container> ChildContainers => _children.Cast<Container>().ToList();

        public List<Widget> ChildWidgets => _children.Cast<Widget>().ToList();

        public List<Control> Children
        {
            get => _children;
            set => _children = value;
        }

        public string DebugLabel
        {
            get => _debugLabel;
            set => _debugLabel = value;
        }

        public float Width
        {
            get => (float)_boundingRectangle.Width;
            set => _boundingRectangle.Width = (int)value;
        }

        public float Height
        {
            get => _boundingRectangle.Height;
            set => _boundingRectangle.Height = (int)value;
        }

        public float XPos
        {
            get => _boundingRectangle.X;
            set => _boundingRectangle.X = (int)value;
        }

        public float YPos
        {
            get => _boundingRectangle.Y;
            set => _boundingRectangle.Y = (int)value;
        }

        public Rectangle BoundingRectangle
        {
            get => _boundingRectangle;
            set => _boundingRectangle = value;
        }

        public AnchorCoord Anchor
        {
            get => anchor;
            set => anchor = value;
        }

        public AnchorType AnchorType
        {
            get => anchor.Type;
            set => anchor.Type = value;
        }

        public float LocalX { get; set; }

        public float LocalY { get; set; }

        public bool IsOpen { get; set; } = true;
        public bool DrawBorder { get; set; } = true;

        public string Tag { get; protected set; }

        public bool IsSticky { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public bool ClipContents = false;

        public int ClipPadding = 1;

        public bool FillParentWidth = false;

        public bool FillParentHeight = false;

        #endregion

        public Color BlendColor { get; set; } = Color.White;

        #region NineSlice

        public bool NineSliceEnabled { get; private set; } = false;

        protected NineSlice NineSlice { get; private set; }

        public void EnableNineSlice(Texture2D nsTx)
        {
            NineSliceEnabled = true;
            DrawBorder = false;
            NineSlice = new NineSlice(nsTx, BoundingRectangle, 1);
            //NineSlice.DrawMode = NSDrawMode.Padded;
        }

        #endregion

        protected Container()
        {
            DebugLabel = "container";
        }

        ~Container()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            try
            {
                Debug.WriteLine("Decoupling Container from parent...");
                Children.ToList().ForEach(c=>c.Dispose());
                Parent.Children.Remove(this);
                Debug.Write(" Done. \n");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to decouple container. Parent was likely null");
                if (e.InnerException != null) UIEventHandler.sendDebugMessage(this, e.InnerException.Message);
            }
        }

        protected Container(Control parent) : this()
        {
            Parent = parent;
        }

        public Container(Control parent, int paddingx, int paddingy, int width, int height,
            AnchorType anchorType = AnchorType.TOPLEFT, string debugLabel = "container") : this(parent)
        {
            DebugLabel = debugLabel;
            LocalX = paddingx;
            LocalY = paddingy;
            Anchor = new AnchorCoord(paddingx, paddingy, anchorType, parent, width, height);
            BoundingRectangle = new Rectangle((int)anchor.AbsolutePosition.X, (int)anchor.AbsolutePosition.Y, width,
                height);
            parent.Add(this);
        }

        public virtual void Update(MouseState oldState, MouseState newState)
        {
            if (!IsOpen || !IsActive)
                return;

            if (IsSticky)
                Anchor.RecalculateAnchor(LocalX, LocalY, Parent, Width, Height);

            BoundingRectangle = new Rectangle(Anchor.AbsolutePosition.ToPoint(), new Point((int)Width, (int)Height));

            if (BoundingRectangle.Contains(newState.Position))
                isUnderMouseFocus = true;
            else isUnderMouseFocus = false;

            if (FillParentWidth)
                Width = parent.Width;
            if (FillParentHeight)
                Height = parent.Height;

            foreach (var child in Children)
                child.Update(oldState, newState);
        }

        public void Remove(Control c)
        {
            _children.ToList().Remove(c);
        }

        public virtual void Draw(SpriteBatch guiSpriteBatch)
        {
            if (!IsOpen)
                return;
            Alpha *= parent.Alpha / 255;
            Rectangle scissor_reset = guiSpriteBatch.GraphicsDevice.ScissorRectangle;
            if (ClipContents)
            {
                Rectangle srect = BoundingRectangle;
                srect.Size += new Point(ClipPadding * 2);
                srect.Location -= new Point(ClipPadding);
                guiSpriteBatch.GraphicsDevice.ScissorRectangle = srect;
            }

            //base.Draw(sb);

            guiSpriteBatch.FillRectangle(BoundingRectangle, Theme.TertiaryColor * (Alpha / 255f));

            if (NineSliceEnabled)
            {
                NineSlice.BindRect = BoundingRectangle;
                NineSlice.Draw(guiSpriteBatch);
            }

            foreach (Control c in _children)
                c.Draw(guiSpriteBatch);
            
            if (DrawBorder)
                guiSpriteBatch.DrawRectangle(BoundingRectangle, Theme.SecondaryColor * (Alpha / 255f));

            if (!IsActive)
                guiSpriteBatch.FillRectangle(BoundingRectangle, Theme.TertiaryColor * (Alpha / 255f) * 0.5f);

            guiSpriteBatch.End();
            guiSpriteBatch.GraphicsDevice.ScissorRectangle = scissor_reset;
            guiSpriteBatch.Begin(rasterizerState: new RasterizerState() { ScissorTestEnable = true });
        }

        public void Add(Widget widget)
        {
            widget.SetParent(this);
            widget.Parent.Remove(widget);
            Children.Add(widget);
        }   
        
        public void PrintChildren(int layer)
        {
            string indent1 = "----";
            string indent = "----";
            for (int i = 0; i < layer; i++)
            {
                indent = indent + indent1;
            }

            foreach (Container container in ChildContainers)
            {
                Debug.WriteLine(indent + container.DebugLabel);
                container.PrintChildren(layer + 1);
            }

            foreach (Widget widget in ChildWidgets)
                Debug.WriteLine(indent + widget.DebugLabel);
        }

        public Control GetParent()
        {
            if (parent == null)
            {
                throw new InvalidOperationException("Instance was not initialised with a parent");
            }

            return parent;
        }

        #region close/open

        public virtual void Close()
        {
            IsOpen = false;
        }

        public virtual void Open()
        {
            IsOpen = true;
        }

        #endregion
        
        public void SetPosition(int x, int y)
        {
            AnchorCoord newAnchor = new AnchorCoord(LocalX, LocalY, AnchorType, Parent, Width, Height)
                { AbsolutePosition = new Vector2(x, y) };

            Anchor = newAnchor;
        }

        public void ResetPosition()
        {
            Anchor = new AnchorCoord(LocalX, LocalY, AnchorType, Parent, Width, Height);
        }

        public void PushToTop(Container c)
        {
            ChildContainers.Remove(c);
            ChildContainers.Insert(0, c);
        }

        public UIRoot FindRoot()
        {
            return Parent.FindRoot();
        }

        public virtual void Click(Vector2 mousePos, ClickMode clickMode, MouseButton buttonType)
        {
            foreach (Control c in Children)
            {
                if (c.IsUnderMouseFocus)
                {
                    c.Click(mousePos, clickMode, buttonType);
                    break;
                }
            }
        }

        public void BorderColor(Color c)
        {
            Theme.SecondaryColor = c;
        }

        public void BackgroundColor(Color c)
        {
            Theme.TertiaryColor = c;
        }

        public Control Add(Control child)
        {
            _children.Add(child);
            return this;
        }
    }
}