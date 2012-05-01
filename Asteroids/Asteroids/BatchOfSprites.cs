using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class BatchOfSprites
    {
        private GraphicsDevice device;
        private const int MaxSprites = 100;
        private SpriteCornerVertex[] spriteVertices = new SpriteCornerVertex[MaxSprites * 6];
        private int firstActiveSprite;
        private int firstFreeSprite;
        private VertexBuffer vertexBuffer;
        private bool isVertexBufferUpToDate = false;

        public BatchOfSprites(GraphicsDevice device)
        {
            this.device = device;
            vertexBuffer = new VertexBuffer(device, SpriteCornerVertex.VertexDeclaration, MaxSprites * 6, BufferUsage.WriteOnly);

            firstActiveSprite = -1;
            firstFreeSprite = 0;
        }

        public void AddSprite(Vector3 position)
        {
            if (firstActiveSprite > 0)
                AddSpriteAtIndex(--firstActiveSprite, position);
            else if (firstFreeSprite < MaxSprites - 1)
            {
                AddSpriteAtIndex(firstFreeSprite++, position);
                if (firstActiveSprite < 0)
                    firstActiveSprite = 0;
            }
            else
                throw new Exception("The batch of sprites cannot contain more sprites.");

            isVertexBufferUpToDate = false;
        }

        public VertexBuffer GetVertexBuffer()
        {
            if (!isVertexBufferUpToDate)
                LoadIntoVertexBuffer(vertexBuffer);
            return vertexBuffer;
        }

        public void DrawAll()
        {
            device.DrawPrimitives(PrimitiveType.TriangleList, firstActiveSprite * 6, GetVertexCount());
        }

        private void AddSpriteAtIndex(int idx, Vector3 position)
        {
            int baseIdx = idx * 6;
            spriteVertices[baseIdx + 0] = new SpriteCornerVertex(new Vector2(-0.5f, -0.5f), position, Color.White, 1.0f, 0.0f);
            spriteVertices[baseIdx + 1] = new SpriteCornerVertex(new Vector2(-0.5f, 0.5f), position, Color.White, 1.0f, 0.0f);
            spriteVertices[baseIdx + 2] = new SpriteCornerVertex(new Vector2(0.5f, 0.5f), position, Color.White, 1.0f, 0.0f);

            spriteVertices[baseIdx + 3] = new SpriteCornerVertex(new Vector2(-0.5f, -0.5f), position, Color.White, 1.0f, 0.0f);
            spriteVertices[baseIdx + 4] = new SpriteCornerVertex(new Vector2(0.5f, 0.5f), position, Color.White, 1.0f, 0.0f);
            spriteVertices[baseIdx + 5] = new SpriteCornerVertex(new Vector2(0.5f, -0.5f), position, Color.White, 1.0f, 0.0f);
        }

        private void LoadIntoVertexBuffer(VertexBuffer vertexBuffer)
        {
            int count = GetVertexCount();
            vertexBuffer.SetData<SpriteCornerVertex>(spriteVertices, firstActiveSprite * 6, count);
            isVertexBufferUpToDate = true;
        }

        private int GetVertexCount()
        {
            return 6 * (firstFreeSprite - firstActiveSprite);
        }
    }
}
