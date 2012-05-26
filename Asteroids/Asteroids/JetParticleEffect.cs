using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class JetParticleEffect : ParticleEffect
    {
        private Random random = new Random();

        public JetParticleEffect(ParticleSystem system)
            : base(system)
        { }

        protected override void SpawnInitialParticles(GameTime time)
        {
            for (int i = 0; i < 10; i++)
            {
                var particle = CreateParticle("sprite");
                particle.Size = 0.1f;
                particle.Position.X = (float)random.Next(1000) / 1000.0f;
                particle.Position.Y = 10.0f;
            }
        }

        protected override bool UpdateParticle(Particle particle, Microsoft.Xna.Framework.GameTime time)
        {
            return true;
        }

        protected override void UpdateSystem(Microsoft.Xna.Framework.GameTime time)
        {
            
        }
    }
}
