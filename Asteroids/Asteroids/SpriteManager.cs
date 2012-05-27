using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class SpriteManager
    {
        private Dictionary<string, BatchOfSprites> batchesByTexture = new Dictionary<string,BatchOfSprites>();
        private Dictionary<Sprite, BatchOfSprites> batchesBySprite = new Dictionary<Sprite, BatchOfSprites>();
        private ContentManager content;
        private GraphicsDevice device;

        public SpriteManager(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            this.content = content;
        }

        public Sprite CreateSprite(string textureName)
        {
            var sprite = new Sprite();
            BatchOfSprites batch;
            if (!batchesByTexture.TryGetValue(textureName, out batch))
            {
                batch = new BatchOfSprites(device);
                batchesByTexture.Add(textureName, batch);
            }
            batch.AddSprite(sprite);
            batchesBySprite.Add(sprite, batch);
            return sprite;
        }

        public void DeleteSprite(Sprite sprite)
        {
            BatchOfSprites batch;
            if (batchesBySprite.TryGetValue(sprite, out batch))
            {
                batch.RemoveSprite(sprite);
                batchesBySprite.Remove(sprite);
            }
        }

        public void DrawAll(SpriteDrawer spriteDrawer, ICamera camera)
        {
            spriteDrawer.Begin(camera);
            foreach (var textureNameBatchPair in batchesByTexture)
            {
                string textureName = textureNameBatchPair.Key;
                BatchOfSprites batch = textureNameBatchPair.Value;
                var texture = content.Load<Texture2D>(textureName);

                spriteDrawer.SetTexture(texture);
                spriteDrawer.DrawBatchOfSprites(batch);
            }
            spriteDrawer.End();
        }
    }
}
