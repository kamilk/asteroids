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

        public SpriteDrawer(GraphicsDevice device)
        {
            this.device = device;
            this.batch = new SpriteBatch(device);
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
            if (!drawingInProgress)
                BeginWithoutSettingFlag();

            var worldMatrix = size != 1.0f ? Matrix.CreateScale(size) : Matrix.Identity;

            var projection = device.Viewport.Project(position, camera.ProjectionMatrix, camera.ViewMatrix, worldMatrix);
            var position2d = new Vector2(projection.X, projection.Y);

            batch.Draw(texture, position2d, color);

            if (!drawingInProgress)
                EndWithoutResettingFlag();
        }

        public void End()
        {
            batch.End();
            drawingInProgress = false;
        }

        private void BeginWithoutSettingFlag()
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        private void EndWithoutResettingFlag()
        {
            batch.End();
        }
    }
}
