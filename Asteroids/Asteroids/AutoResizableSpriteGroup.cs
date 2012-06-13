using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class AutoResizableSpriteGroup
    {
        private GraphicsDevice device;
        private HashSet<BatchOfSprites> batches = new HashSet<BatchOfSprites>();
        private Dictionary<Sprite, BatchOfSprites> batchesBySprite = new Dictionary<Sprite, BatchOfSprites>();

        public AutoResizableSpriteGroup(GraphicsDevice device)
        {
            this.device = device;
        }

        public void AddSprite(Sprite sprite)
        {
            BatchOfSprites batch = batches.FirstOrDefault(b => !b.IsFull);
            if (batch == null)
            {
                batch = new BatchOfSprites(device);
                batches.Add(batch);
            }
            batch.AddSprite(sprite);
            batchesBySprite.Add(sprite, batch);
        }

        public void RemoveSprite(Sprite sprite)
        {
            BatchOfSprites batch;
            if (batchesBySprite.TryGetValue(sprite, out batch))
            {
                batch.RemoveSprite(sprite);
                batchesBySprite.Remove(sprite);
                if (batch.IsEmpty)
                    batches.Remove(batch);
            }
        }

        public void DrawAll()
        {
            foreach (BatchOfSprites batch in batches)
            {
                device.SetVertexBuffer(batch.GetVertexBuffer());
                batch.DrawAll();
            }
        }
    }
}
