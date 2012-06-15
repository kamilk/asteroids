using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public class SpriteManager : IDisposable
    {
        private Dictionary<TextureAndMaskNames, AutoResizableSpriteGroup> spriteGroupsByTexture = new Dictionary<TextureAndMaskNames, AutoResizableSpriteGroup>();
        private Dictionary<Sprite, AutoResizableSpriteGroup> spriteGroupsBySprite = new Dictionary<Sprite, AutoResizableSpriteGroup>();
        private ContentManager content;
        private GraphicsDevice device;

        public SpriteManager(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            this.content = content;
        }

        public Sprite CreateSprite(string textureName, string maskName)
        {
            var textureAndMaskNames = new TextureAndMaskNames(textureName, maskName);

            var sprite = new Sprite();
            AutoResizableSpriteGroup spriteGroup;
            if (!spriteGroupsByTexture.TryGetValue(textureAndMaskNames, out spriteGroup))
            {
                spriteGroup = new AutoResizableSpriteGroup(device);
                spriteGroupsByTexture.Add(textureAndMaskNames, spriteGroup);
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
                TextureAndMaskNames textureName = textureNameSpriteGroupPair.Key;
                AutoResizableSpriteGroup spriteGroup = textureNameSpriteGroupPair.Value;
                var texture = content.Load<Texture2D>(textureName.TextureName);
                var maskTexture = content.Load<Texture2D>(textureName.MaskTextureName);

                spriteDrawer.SetTexture(texture);
                spriteDrawer.SetMaskTexture(maskTexture);
                spriteDrawer.DrawBatchOfSprites(spriteGroup);
            }
            spriteDrawer.End();
        }

        public void Dispose()
        {
            foreach (var group in spriteGroupsByTexture.Values)
                group.Dispose();
        }
    }
}
