using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class Particle
    {
        public float Size
        {
            get { return Sprite.Size; }
            set { Sprite.Size = value; }
        }

        public ObservableVector3 Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position.Set(value); }
        }

        public float Rotation
        {
            get { return Sprite.Rotation; }
            set { Sprite.Rotation = value; }
        }

        public Color Color
        {
            get { return Sprite.Color; }
            set { Sprite.Color = value; }
        }

        public Sprite Sprite
        {
            get;
            private set;
        }

        public Particle(Sprite sprite)
        {
            this.Sprite = sprite;
        }
    }
}
