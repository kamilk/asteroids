using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    abstract class ParticleEffect
    {
        HashSet<Particle> particles = new HashSet<Particle>();
        bool initialized = false;

        public ParticleSystem ParticleSystem
        {
            get;
            private set;
        }

        public ParticleEffect(ParticleSystem system)
        {
            this.ParticleSystem = system;
        }

        public void Update(GameTime time)
        {
            if (!initialized)
            {
                SpawnInitialParticles(time);
                initialized = true;
            }

            UpdateSystem(time);

            List<Particle> particlesToDelete = new List<Particle>();
            foreach (Particle particle in particles)
            {
                if (!UpdateParticle(particle, time))
                    particlesToDelete.Add(particle);
            }

            foreach (Particle particle in particlesToDelete)
            {
                ParticleSystem.DeleteParticle(particle);
                particles.Remove(particle);
            }
        }

        protected abstract void SpawnInitialParticles(GameTime time);

        protected abstract bool UpdateParticle(Particle particle, GameTime time);

        protected abstract void UpdateSystem(GameTime time);

        protected Particle CreateParticle(string textureName, GameTime time)
        {
            var particle = ParticleSystem.CreateParticle(textureName);
            particles.Add(particle);
            particle.SpawnTime = time.TotalGameTime;
            return particle;
        }
    }
}
