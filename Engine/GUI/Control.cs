using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGui.Engine.GUI
{
    public interface Control
    {
        bool IsUnderMouseFocus { get; }
        List<Control> Children { get; }
        public Control Add(Control c);
        public void Remove(Control c);
        internal void Click(Vector2 mousePosition, ClickMode clickMode, MouseButton buttonType);
        internal void Draw(SpriteBatch sb);
        internal void Update(MouseState oldState, MouseState newState);
        float LocalX { get; set; }
        float LocalY { get; set; }

        float Width { get; set; }
        float Height { get; set; }

        float XPos { get; }
        float YPos { get; }
        AnchorCoord Anchor { get; }
        float Alpha { get; set; }
        UIRoot FindRoot();
        void Dispose();
    }
}
