using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Interfejs kamery.
    /// </summary>
    public interface ICamera
    {
        Matrix ProjectionMatrix
        {
            get;
        }
        Matrix ViewMatrix
        {
            get;
        }

        void Update();
    }
}
