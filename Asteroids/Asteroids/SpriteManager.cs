using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class SpriteManager
    {
        private Dictionary<string, AutoResizableSpriteGroup> spriteGroupsByTexture = new Dictionary<string, AutoResizableSpriteGroup>();
        private Dictionary<Sprite, AutoResizableSpriteGroup> spriteGroupsBySprite = new Dictionary<Sprite, AutoResizableSpriteGroup>();
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
            AutoResizableSpriteGroup spriteGroup;
            if (!spriteGroupsByTexture.TryGetValue(textureName, out spriteGroup))
            {
                spriteGroup = new AutoResizableSpriteGroup(device);
                spriteGroupsByTexture.Add(textureName, spriteGroup);
            }
            spriteGroup.AddSprite(sprite);
            spriteGroupsBySprite.Add(sprite, spriteGroup);
            return sprite;
        }

        public void DeleteSprite(Sprite sprite)
        {
            AutoResizableSpriteGroup spriteGroup;
            if (spriteGroupsBySprite.TryGetValue(sprite, out spriteGroup))
            {
                spriteGroup.RemoveSprite(sprite);
                spriteGroupsBySprite.Remove(sprite);
            }
        }

        public void DrawAll(SpriteDrawer spriteDrawer, ICamera camera)
        {
            spriteDrawer.Begin(camera);
            foreach (var textureNameSpriteGroupPair in spriteGroupsByTexture)
            {
                string textureName = textureNameSpriteGroupPair.Key;
                AutoResizableSpriteGroup spriteGroup = textureNameSpriteGroupPair.Value;
                var texture = content.Load<Texture2D>(textureName);

                spriteDrawer.SetTexture(texture);
                spriteDrawer.DrawBatchOfSprites(spriteGroup);
            }
            spriteDrawer.End();
        }
    }
}
