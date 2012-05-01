using Microsoft.Xna.Framework;

namespace Asteroids
{
    class SpriteData
    {
        public Vector3 Position;
        public Color Color;
        public float Size;
        public float Rotation;

        public SpriteData(Vector3 position, float size, Color color, float rotation = 0.0f)
        {
            this.Position = position;
            this.Color = color;
            this.Size = size;
            this.Rotation = rotation;
        }

        public SpriteData(Vector3 position, float size)
            : this(position, size, Color.White)
        { }
    }
}
