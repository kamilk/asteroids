using Microsoft.Xna.Framework;

namespace Asteroids
{
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
        Vector3 Position
        {
            get;
            set;
        }
        void Update();
    }
}
