﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    public interface IModel
    {
        Model Model
        {
            get;
        }

        Quaternion Rotation
        {
            get;
        }

        Vector3 Position
        {
            get;
        }

        Matrix WorldMatrix
        {
            get;
            set;
        }

        float Scale
        {
            get;
        }

        Matrix[] Transforms
        {
            get;
        }

        Spaceship Ship
        {
            get;
        }

        void Update();
        void Draw(ICamera fpsCam);

    }
}