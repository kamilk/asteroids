using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Interfejs trójwymiarowego obiektu.
    /// </summary>
    public interface IModel : IPositionedObject
    {
        Model Model
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

        void Update(GameTime time, Vector3 centerOfUniverse);
        void Draw(ICamera fpsCam);

    }
}
