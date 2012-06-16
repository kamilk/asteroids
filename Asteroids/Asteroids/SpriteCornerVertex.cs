using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    /// <summary>
    /// Struktura wierzchołka Sprite'a.
    /// </summary>
    struct SpriteCornerVertex
    {
        public Vector2 Corner;
        public Vector3 ParticleCenter;
        public Color Color;
        public float Size;
        public float Rotation;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
            new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
        );

        public SpriteCornerVertex(Vector2 corner, Vector3 particleCenter, Color color, float size, float rotation)
        {
            this.Corner = corner;
            this.ParticleCenter = particleCenter;
            this.Color = color;
            this.Size = size;
            this.Rotation = rotation;
        }
    }
}
