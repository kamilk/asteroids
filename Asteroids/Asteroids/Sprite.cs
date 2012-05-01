using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class Sprite
    {
        public Vector3 Position { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public float Rotation { get; set; }

        public Sprite(Vector3 position, float size, Color color, float rotation = 0.0f)
        {
            this.Position = position;
            this.Color = color;
            this.Size = size;
            this.Rotation = rotation;
        }

        public Sprite(Vector3 position, float size)
            : this(position, size, Color.White)
        { }

        public override int GetHashCode()
        {
            return Color.GetHashCode() + Position.GetHashCode()
                + Rotation.GetHashCode() + Size.GetHashCode();
        }
    }
}
