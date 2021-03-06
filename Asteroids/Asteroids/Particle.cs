﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Klasa reprezentująca cząstkę efektu cząsteczkowego.
    /// </summary>
    public class Particle
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

        public Vector3 Velocity
        {
            get;
            set;
        }

        public Sprite Sprite
        {
            get;
            private set;
        }

        public TimeSpan SpawnTime
        {
            get;
            set;
        }

        public Particle(Sprite sprite)
        {
            this.Sprite = sprite;
            this.Velocity = Vector3.Zero;
        }
    }
}
