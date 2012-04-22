using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class SpriteDrawer : IDisposable
    {
        private GraphicsDevice device;
        private SpriteBatch batch;
        private bool drawingInProgress = false;
        private BasicEffect effect;

        public SpriteDrawer(GraphicsDevice device)
        {
            this.device = device;
            
            batch = new SpriteBatch(device);
            effect = new BasicEffect(device);
            effect.TextureEnabled = true;
        }

        public void Dispose()
        {
            batch.Dispose();
        }

        public void Begin()
        {
            BeginWithoutSettingFlag();
            drawingInProgress = true;
        }

        public void DrawSprite(ICamera camera, Vector3 position, Texture2D texture, float size, Color color)
        {
            effect.World = Matrix.CreateTranslation(position) * Matrix.CreateScale(size);
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;

            batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, effect);
            batch.Draw(texture, Vector2.Zero, color);
            batch.End();
        }

        public void End()
        {
            batch.End();
            drawingInProgress = false;
        }

        private void BeginWithoutSettingFlag()
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect);
        }

        private void EndWithoutResettingFlag()
        {
            batch.End();
        }
    }
}
