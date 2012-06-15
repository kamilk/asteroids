using System;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class MissileJetParticleEffect : ParticleEffect
    {
        private const double particlesPerSecond = 30.0;
        private const double particleLifetime = 2000.0;

        private double nextSpawnTime;
        private Missile missile;

        private bool active = true;

        public bool IsDead
        {
            get { return !active && !HasAnyParticles; }
        }

        public MissileJetParticleEffect(ParticleSystem system, Missile missile)
            : base(system)
        {
            this.missile = missile;
        }

        public void StopSpawningParticles()
        {
            active = false;
        }

        protected override void OnBeforeFirstUpdate(GameTime time)
        {
            nextSpawnTime = time.TotalGameTime.TotalMilliseconds + 1000.0f / particlesPerSecond;
        }

        protected override void UpdateSystem(GameTime time)
        {
            if (!active)
                return;

            if (!ShouldNewParticleSpawn(time))
                return;

            do
            {
                nextSpawnTime += 1000.0f / particlesPerSecond;

                Vector3 jetDirection = new Vector3(
                    AsteroidsUtilities.Random(0.6f, 0.8f),
                    AsteroidsUtilities.Random(-0.025f, 0.025f),
                    AsteroidsUtilities.Random(-0.025f, 0.025f));

                Vector3 velocity = Vector3.Transform(jetDirection, missile.Rotation);
                velocity.Normalize();
                velocity *= 5.0f;

                var particle = CreateParticle(ResourceNames.ParticleTexture, ResourceNames.ParticleMask, time);
                particle.Position.UnderlyingVector = missile.Position;
                particle.Velocity = velocity;
                particle.Size = 0.2f;
            } while (ShouldNewParticleSpawn(time));
        }

        protected override bool UpdateParticle(Particle particle, GameTime time)
        {
            if (particle.SpawnTime.TotalMilliseconds + particleLifetime < time.TotalGameTime.TotalMilliseconds)
                return false;

            particle.Position.UnderlyingVector += particle.Velocity * (float)time.ElapsedGameTime.TotalSeconds;

            return true;
        }

        private bool ShouldNewParticleSpawn(GameTime time)
        {
            return nextSpawnTime < time.TotalGameTime.TotalMilliseconds;
        }
    }
}
