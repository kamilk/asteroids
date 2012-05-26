using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class Particle
    {
        Sprite sprite;

        public float Size
        {
            get { return sprite.Size; }
            set { sprite.Size = value; }
        }

        public ObservableVector3 Position
        {
            get { return sprite.Position; }
            set { sprite.Position.Set(value); }
        }

        public float Rotation
        {
            get { return sprite.Rotation; }
            set { sprite.Rotation = value; }
        }

        public Color Color
        {
            get { return sprite.Color; }
            set { sprite.Color = value; }
        }

        public Particle()
        {
            this.sprite = new Sprite(Vector3.Zero, 1.0f);
        }

        internal Sprite GetSprite()
        {
            return sprite;
        }
    }
}
