using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    /// <summary>
    /// Klasa zawierająca statyczne metody, jakie mogą się przydać 
    /// gdziekolwiek w naszym programie.
    /// </summary>
    static class AsteroidsUtilities
    {
        private static Random random = new Random();

        public static float Random(float min, float max)
        {
            return min + (max - min) * (float)random.Next(1000000) / 1000000.0f;
        }
    }
}
