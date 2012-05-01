using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class Sprite
    {
        private BatchOfSprites parent;

        public SpriteData SpriteData
        {
            get;
            private set;
        }

        public Sprite(BatchOfSprites batch, SpriteData data)
        {
            this.parent = batch;
            this.SpriteData = data;
        }

        public override int GetHashCode()
        {
            return SpriteData.Color.GetHashCode() + SpriteData.Position.GetHashCode()
                + SpriteData.Rotation.GetHashCode() + SpriteData.Size.GetHashCode();
        }

        public void DetachFromParent()
        {
            parent = null;
        }
    }
}
