using System.Collections.Generic;

namespace Asteroids
{
    public class ParticleSystem
    {
        private SpriteManager spriteManager;

        public ParticleSystem(SpriteManager spriteManager)
        {
            this.spriteManager = spriteManager;
        }

        public Particle CreateParticle(string textureName, string maskName)
        {
            Sprite sprite = spriteManager.CreateSprite(textureName, maskName);
            return new Particle(sprite);
        }

        public void DeleteParticle(Particle particle)
        {
            spriteManager.DeleteSprite(particle.Sprite);
        }
    }
}
