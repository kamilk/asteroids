using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Interfejs obiektu, który może znajdować się w jakimś miejscu na ekranie
    /// i być zorientowany w przestrzeni trójwymiarowej. 
    /// </summary>
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
