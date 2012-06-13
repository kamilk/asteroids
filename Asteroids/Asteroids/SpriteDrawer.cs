using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    public class SpriteDrawer : IDisposable
    {
        private GraphicsDevice device;
        private IndexBuffer indexBuffer;
        private Effect effect;
        private RasterizerState rasterizerState;
        private RasterizerState oldRasterizerState;
        private bool drawingInProgress;

        public SpriteDrawer(GraphicsDevice device, ContentManager content)
        {
            this.device = device;

            int spritesInBatch = BatchOfSprites.MaxSprites;
            int verticesInBatch = spritesInBatch * 6;
            indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, verticesInBatch, BufferUsage.WriteOnly);

            var indices = new ushort[verticesInBatch];
            for (int i = 0; i < spritesInBatch; i++)
            {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);

                indices[i * 6 + 3] = (ushort)(i * 4 + 0);
                indices[i * 6 + 4] = (ushort)(i * 4 + 2);
                indices[i * 6 + 5] = (ushort)(i * 4 + 3);
            }
            indexBuffer.SetData<ushort>(indices);

            effect = content.Load<Effect>("SpriteEffect");

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
        }

        public void Dispose()
        {
            indexBuffer.Dispose();
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
            effect.Parameters["ViewportScale"].SetValue(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));

            oldRasterizerState = device.RasterizerState;
            device.RasterizerState = rasterizerState;

            device.Indices = indexBuffer;
        }

        public void SetTexture(Texture2D texture)
        {
            effect.Parameters["Texture"].SetValue(texture);
        }

        public void SetMaskTexture(Texture2D texture)
        {
            effect.Parameters["MaskTexture"].SetValue(texture);
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

        public void DrawBatchOfSprites(AutoResizableSpriteGroup spriteGroup)
        {
            effect.Parameters["World"].SetValue(Matrix.Identity);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteGroup.DrawAll();
            }
        }
    }
}
