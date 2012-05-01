using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
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
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.PointSize, 0),
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.PointSize, 1)
        );
    }
}
