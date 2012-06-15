using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Interfejs obiektu, który posiada silnik odrzutowy. Zawiera właściwości, jakie są 
    /// potrzebne aby móc wyrzucić cząstki z tego silnika - tj. nadać im początkowe 
    /// położenie i kierunek.
    /// </summary>
    public interface IJet : IPositionedObject
    {
        Vector3 JetPosition { get; }
    }
}
