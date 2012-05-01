﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class SpriteDrawer : IDisposable
    {
        private GraphicsDevice device;
        private VertexBuffer vertexBuffer;
        private Effect effect;
        private RasterizerState rasterizerState;
        private RasterizerState oldRasterizerState;
        private bool drawingInProgress;
        
        public SpriteDrawer(GraphicsDevice device, ContentManager content)
        {
            this.device = device;

            var vertices = new VertexPositionColorTexture[6];
            vertices[0] = new VertexPositionColorTexture(new Vector3(-0.5f, -0.5f, 0.0f), Color.White, new Vector2(0.0f, 0.0f));
            vertices[1] = new VertexPositionColorTexture(new Vector3(-0.5f, 0.5f, 0.0f), Color.White, new Vector2(0.0f, 1.0f));
            vertices[2] = new VertexPositionColorTexture(new Vector3(0.5f, 0.5f, 0.0f), Color.White, new Vector2(1.0f, 1.0f));

            vertices[3] = new VertexPositionColorTexture(new Vector3(-0.5f, -0.5f, 0.0f), Color.White, new Vector2(0.0f, 0.0f));
            vertices[4] = new VertexPositionColorTexture(new Vector3(0.5f, 0.5f, 0.0f), Color.White, new Vector2(1.0f, 1.0f));
            vertices[5] = new VertexPositionColorTexture(new Vector3(0.5f, -0.5f, 0.0f), Color.White, new Vector2(1.0f, 0.0f));

            vertexBuffer = new VertexBuffer(device, VertexPositionColorTexture.VertexDeclaration, 6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(vertices);

            effect = content.Load<Effect>("SpriteEffect");

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }

        public void Begin(ICamera camera)
        {
            BeginWithoutSettingFlag(camera);
            drawingInProgress = true;
        }

        private void BeginWithoutSettingFlag(ICamera camera)
        {
            effect.Parameters["View"].SetValue(camera.ViewMatrix);
            effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);

            oldRasterizerState = device.RasterizerState;
            device.RasterizerState = rasterizerState;

            device.SetVertexBuffer(vertexBuffer);
        }

        public void SetTexture(Texture2D texture)
        {
            effect.Parameters["Texture"].SetValue(texture);
        }

        public void End()
        {
            EndWithoutResettingFlag();
            drawingInProgress = false;
        }

        private void EndWithoutResettingFlag()
        {
            device.RasterizerState = oldRasterizerState;
        }

        public void DrawSprite(ICamera camera, Vector3 position, float size, Color color)
        {
            if (!drawingInProgress)
                throw new Exception("DrawSprite() was called before Begin()");

            effect.Parameters["World"].SetValue(Matrix.CreateTranslation(position) * Matrix.CreateScale(size));

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }
    }
}
