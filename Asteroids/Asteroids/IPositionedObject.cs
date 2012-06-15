using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    public interface IPositionedObject
    {
        Quaternion Rotation
        {
            get;
        }

        Vector3 Position
        {
            get;
        }
    }
}
