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

        public bool HasAnyParticles
        {
            get { return particles.Count > 0; }
        }

        public ParticleEffect(ParticleSystem system)
        {
            this.ParticleSystem = system;
        }

        public void Update(GameTime time)
        {
            if (!initialized)
            {
                OnBeforeFirstUpdate(time);
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

        protected abstract void OnBeforeFirstUpdate(GameTime time);

        protected abstract bool UpdateParticle(Particle particle, GameTime time);

        protected abstract void UpdateSystem(GameTime time);

        protected Particle CreateParticle(string textureName, string maskName, GameTime time)
        {
            var particle = ParticleSystem.CreateParticle(textureName, maskName);
            particles.Add(particle);
            particle.SpawnTime = time.TotalGameTime;
            return particle;
        }
    }
}
