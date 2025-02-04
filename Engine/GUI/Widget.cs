﻿using MonoGui.Designer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGui.Engine.GUI
{
    public class Widget : Control
    {
        protected string debug_label;
        protected AnchorCoord anchor;
        protected Rectangle bounding_rectangle;
        protected bool isUnderMouseFocus;
        protected Control _parent;
        protected Action<Rectangle> _clickCallback;

        public bool DrawDebugRect { get; set; } = false;

        public bool Enabled { get; set; } = true;

        public Control Parent { get => _parent; protected set => _parent = value; }

        public Vector2 AbsolutePosition { get => anchor.AbsolutePosition; }

        public string DebugLabel { get => debug_label; set => debug_label = value; }

        public AnchorCoord Anchor { get => anchor; protected set => anchor = value; }

        public Rectangle BoundingRectangle { get => bounding_rectangle; protected set => bounding_rectangle = value; }

        public float XPos { get => bounding_rectangle.X; set => bounding_rectangle.X = (int)value; }

        public float YPos { get => bounding_rectangle.Y; set => bounding_rectangle.Y = (int)value; }

        public float LocalX { get; set; }

        public float LocalY { get; set; }

        public float Width { get => bounding_rectangle.Width; set => bounding_rectangle.Width = (int)value; }

        public float Height { get => bounding_rectangle.Height; set => bounding_rectangle.Height = (int)value; }

        public Vector2 localOrigin { get; set; }

        public bool IsUnderMouseFocus { get => isUnderMouseFocus; }

        public AnchorType anchorType { get => anchor.Type; set => anchor.Type = value; }
        public LocalThemeProperties Theme = new LocalThemeProperties();
        private readonly List<Control> _children = new List<Control>();
        public float Alpha { get; set; } = 255f;
        public UIRoot FindRoot()
        {
            return Parent.FindRoot();
        }

        protected Widget(Container parent)
        {
            Parent = parent;
            Alpha = 255f;
        }
        public Widget() { }

        ~Widget()
        {
            Dispose();
        }
        public virtual void Dispose() { }

        public Widget(Control parent, int width, int height, int relativex = 10, int relativey = 10, AnchorType anchorType = AnchorType.TOPLEFT, string debugLabel = "widget")
        {
            LocalX = relativex;
            LocalY = relativey;
            DebugLabel = debugLabel;
            Parent = parent;

            localOrigin = new Vector2(width / 2, height / 2);
            Anchor = new AnchorCoord(LocalX, LocalY, anchorType, parent, width, height);
            BoundingRectangle = new Rectangle((int)Anchor.AbsolutePosition.X, (int)Anchor.AbsolutePosition.Y, width, height);
            parent.Add(this);
            UpdatePos();
        }

        public List<Control> Children => _children;

        public Control Add(Control c)
        {
            _children.Add(c);
            return this;
        }

        public void Remove(Control c)
        {
            _children.ToList().Remove(c);
        }

        public virtual void Draw(SpriteBatch guiSpriteBatch)
        {
            if (!Enabled)
                return;
            if (DrawDebugRect)
            {
                guiSpriteBatch.DrawRectangle(BoundingRectangle, Theme.PrimaryColor * (Alpha / 255f));
                guiSpriteBatch.FillRectangle(BoundingRectangle, Theme.PrimaryColor * 0.5f * (Alpha / 255f));
            }
        }

        public virtual void Update(MouseState oldState, MouseState newState)
        {
            if (!Enabled)
                return;
            Alpha *= Parent.Alpha / 255;
            if (BoundingRectangle.Contains(newState.Position))
                isUnderMouseFocus = true;
            else isUnderMouseFocus = false;
            // change stuff here, position, etc. it will then be updated by the function below.            
            UpdatePos();
        }
        /// <summary>
        /// Transfer over to a new parent - best not to use on its own. Called whenever you want to "AddNewWidget" on a container.
        /// </summary>
        /// <param name="newParent"></param>
        internal void SetParent(Control newParent)
        {
            Parent = newParent;
        }

        public virtual void UpdatePos()
        {
            Anchor.RecalculateAnchor(LocalX, LocalY, Parent, Width, Height);
            bounding_rectangle = new Rectangle((int)anchor.AbsolutePosition.X, (int)anchor.AbsolutePosition.Y, (int)Width, (int)Height);
        }

        public virtual void Click(Vector2 mousePosition, ClickMode clickMode, MouseButton buttonType)
        {
            _clickCallback?.Invoke(BoundingRectangle);
        }

        public Widget OnClick(Action<Rectangle> a)
        {
            _clickCallback = a;
            return this;
        }
        public Widget Tooltip(string content)
        {
            new Tooltip(FindRoot(), this, content);
            return this;
        }
    }
}
