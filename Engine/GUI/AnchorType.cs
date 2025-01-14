﻿using Microsoft.Xna.Framework;

namespace MonoGui.Engine.GUI
{
    public enum AnchorType
    {
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT,
        CENTRE
    }

    public class AnchorCoord
    {
        public AnchorCoord(float XOffset, float YOffset, AnchorType anchorType, Control parent, float width, float height)
        {
            Type = anchorType;
            OffsetFromAnchor = new Vector2();
            AbsolutePosition = new Vector2();
            AnchorLocation = new Vector2();
            switch (anchorType)
            {
                case AnchorType.TOPLEFT:
                    AnchorLocation = new Vector2(parent.XPos, parent.YPos);
                    OffsetFromAnchor = new Vector2(XOffset, YOffset);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.TOPRIGHT:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width, parent.YPos);
                    OffsetFromAnchor = new Vector2(XOffset - width, YOffset);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.BOTTOMLEFT:
                    AnchorLocation = new Vector2(parent.XPos, parent.YPos + parent.Height);
                    OffsetFromAnchor = new Vector2(XOffset, YOffset - height);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.BOTTOMRIGHT:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width, parent.YPos + parent.Height);
                    OffsetFromAnchor = new Vector2(XOffset - width, YOffset - height);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.CENTRE:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width / 2, parent.YPos + parent.Height / 2);
                    OffsetFromAnchor = new Vector2(XOffset - width / 2, YOffset - height / 2);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
            }
            DistanceFromOrigin = new Vector2(AbsolutePosition.X - parent.XPos, AbsolutePosition.Y - parent.YPos);
        }

        public Vector2 AnchorLocation { get; private set; }
        public Vector2 OffsetFromAnchor { get; set; }

        public Vector2 DistanceFromOrigin { get; private set; }

        public Vector2 AbsolutePosition { get; set; }
        public AnchorType Type { get; set; }

        public void RecalculateAnchor(float XOffset, float YOffset, Control parent, float width, float height)
        {
            OffsetFromAnchor = new Vector2();
            AbsolutePosition = new Vector2();
            AnchorLocation = new Vector2();
            switch (Type)
            {
                case AnchorType.TOPLEFT:
                    AnchorLocation = new Vector2(parent.XPos, parent.YPos);
                    OffsetFromAnchor = new Vector2(XOffset, YOffset);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.TOPRIGHT:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width, parent.YPos);
                    OffsetFromAnchor = new Vector2(XOffset - width, YOffset);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.BOTTOMLEFT:
                    AnchorLocation = new Vector2(parent.XPos, parent.YPos + parent.Height);
                    OffsetFromAnchor = new Vector2(XOffset, YOffset - height);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.BOTTOMRIGHT:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width, parent.YPos + parent.Height);
                    OffsetFromAnchor = new Vector2(XOffset - width, YOffset - height);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;
                case AnchorType.CENTRE:
                    AnchorLocation = new Vector2(parent.XPos + parent.Width / 2, parent.YPos + parent.Height / 2);
                    OffsetFromAnchor = new Vector2(XOffset - width / 2, YOffset - height / 2);
                    AbsolutePosition = AnchorLocation + OffsetFromAnchor;
                    break;

            }
            DistanceFromOrigin = new Vector2(AbsolutePosition.X - parent.XPos, AbsolutePosition.Y - parent.YPos);
        }
    }
}

