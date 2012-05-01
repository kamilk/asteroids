using System;
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

            vertexBuffer = new VertexBuffer(device, VertexPositionColorTexture.VertexDeclaration, 6, BufferUsage.WriteOnly);

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

        public void DrawBatchOfSprites(BatchOfSprites batch)
        {
            device.SetVertexBuffer(batch.GetVertexBuffer());

            effect.Parameters["World"].SetValue(Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                batch.DrawAll();
            }
        }
    }
}
