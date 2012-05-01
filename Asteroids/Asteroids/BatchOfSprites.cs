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
        public const int MaxSprites = 100;

        private GraphicsDevice device;
        private Dictionary<Sprite, int> spriteIndices = new Dictionary<Sprite, int>();
        private SpriteCornerVertex[] spriteVertices = new SpriteCornerVertex[MaxSprites * 4];
        private int firstActiveSprite;
        private int firstFreeSprite;
        private VertexBuffer vertexBuffer;
        private bool isVertexBufferUpToDate = false;

        public BatchOfSprites(GraphicsDevice device)
        {
            this.device = device;
            vertexBuffer = new VertexBuffer(device, SpriteCornerVertex.VertexDeclaration, MaxSprites * 4, BufferUsage.WriteOnly);

            firstActiveSprite = -1;
            firstFreeSprite = 0;
        }

        public void AddSprite(Sprite sprite)
        {
            int index;
            if (firstActiveSprite > 0)
            {
                index = --firstActiveSprite;
            }
            else if (firstFreeSprite < MaxSprites - 1)
            {
                index = firstFreeSprite++;
                AddSpriteAtIndex(index, sprite);
                if (firstActiveSprite < 0)
                    firstActiveSprite = 0;
            }
            else
                throw new Exception("The batch of sprites cannot contain more sprites.");

            AddSpriteAtIndex(index, sprite);
            isVertexBufferUpToDate = false;

            spriteIndices.Add(sprite, index);
        }

        public void RemoveSprite(Sprite sprite)
        {
            try
            {
                int index = spriteIndices[sprite];
                spriteIndices.Remove(sprite);

                //if we're removing the first or the last sprite in the vertex buffer, 
                //we may simply do it by moving pointers. If we're removing a sprite from
                //the middle of the buffer, we will need to rebuild the vertex buffer
                if (index == firstActiveSprite)
                    firstActiveSprite++;
                else if (index == firstFreeSprite - 1)
                    firstFreeSprite--;
                else
                    RebuildVertexArray();
            }
            catch (KeyNotFoundException)
            { }
        }

        private void RebuildVertexArray()
        {
            int index = 0;
            var newSpriteIndices = new Dictionary<Sprite, int>();
            foreach (var sprite in spriteIndices.Keys)
            {
                AddSpriteAtIndex(index, sprite);
                newSpriteIndices[sprite] = index;
                index++;
            }
            spriteIndices = newSpriteIndices;
            isVertexBufferUpToDate = false;
            firstActiveSprite = spriteIndices.Count == 0 ? -1 : 0;
            firstFreeSprite = index;
        }

        public VertexBuffer GetVertexBuffer()
        {
            if (!isVertexBufferUpToDate)
                LoadIntoVertexBuffer();
            return vertexBuffer;
        }

        public void DrawAll()
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, GetVertexCount(), 0, GetTriangleCount());
        }

        private void AddSpriteAtIndex(int idx, Sprite sprite)
        {
            int baseIdx = idx * 4;
            Vector3 position = sprite.Position;
            Color color = sprite.Color;
            float size = sprite.Size;
            float rotation = sprite.Rotation;
            spriteVertices[baseIdx + 0] = new SpriteCornerVertex(new Vector2(-0.5f, -0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 1] = new SpriteCornerVertex(new Vector2(-0.5f, 0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 2] = new SpriteCornerVertex(new Vector2(0.5f, 0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 3] = new SpriteCornerVertex(new Vector2(0.5f, -0.5f), position, color, size, rotation);
        }

        private void LoadIntoVertexBuffer()
        {
            int count = GetVertexCount();
            vertexBuffer.SetData<SpriteCornerVertex>(spriteVertices, firstActiveSprite * 4, count);
            isVertexBufferUpToDate = true;
        }

        private int GetVertexCount()
        {
            return 4 * (firstFreeSprite - firstActiveSprite);
        }

        private int GetTriangleCount()
        {
            return 2 * (firstFreeSprite - firstActiveSprite);
        }
    }
}
