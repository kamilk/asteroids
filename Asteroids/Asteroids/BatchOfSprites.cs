﻿using System;
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
        private SpriteCornerVertex[] spriteVertices = new SpriteCornerVertex[MaxSprites * 6];
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

        public void AddSprite(SpriteData spriteData)
        {
            if (firstActiveSprite > 0)
                AddSpriteAtIndex(--firstActiveSprite, spriteData);
            else if (firstFreeSprite < MaxSprites - 1)
            {
                AddSpriteAtIndex(firstFreeSprite++, spriteData);
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
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, GetVertexCount(), 0, GetTriangleCount());
        }

        private void AddSpriteAtIndex(int idx, SpriteData spriteData)
        {
            int baseIdx = idx * 4;
            Vector3 position = spriteData.Position;
            Color color = spriteData.Color;
            float size = spriteData.Size;
            float rotation = spriteData.Rotation;
            spriteVertices[baseIdx + 0] = new SpriteCornerVertex(new Vector2(-0.5f, -0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 1] = new SpriteCornerVertex(new Vector2(-0.5f, 0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 2] = new SpriteCornerVertex(new Vector2(0.5f, 0.5f), position, color, size, rotation);
            spriteVertices[baseIdx + 3] = new SpriteCornerVertex(new Vector2(0.5f, -0.5f), position, color, size, rotation);
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

        private int GetTriangleCount()
        {
            return 2 * (firstFreeSprite - firstActiveSprite);
        }
    }
}
