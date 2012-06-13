using System;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    class JetParticleEffect : ParticleEffect
    {
        private const double particlesPerSecond = 30.0;
        private const double particleLifetime = 2000.0;

        private Random random = new Random();
        private double nextSpawnTime;
        private Spaceship ship;

        public JetParticleEffect(ParticleSystem system, Spaceship ship)
            : base(system)
        {
            this.ship = ship;
        }

        protected override void OnBeforeFirstUpdate(GameTime time)
        {
            nextSpawnTime = time.TotalGameTime.TotalMilliseconds + 1000.0f / particlesPerSecond;
        }

        protected override void UpdateSystem(GameTime time)
        {
            if (!ShouldNewParticleSpawn(time))
                return;

            Matrix jetMatrix = ship.GetJetOrientationMatrix();
            Vector3 position = Vector3.Transform(Vector3.Zero, jetMatrix);

            do
            {
                nextSpawnTime += 1000.0f / particlesPerSecond;

                Vector3 jetDirection = new Vector3(Random(-0.1f, 0.1f), Random(-0.1f, 0.1f), Random(0.6f, 0.8f));
                Vector3 velocity = Vector3.Transform(jetDirection, jetMatrix) - position;
                velocity.Normalize();
                velocity *= 0.5f;

                var particle = CreateParticle(ResourceNames.ParticleTexture, ResourceNames.ParticleMask, time);
                particle.Position.UnderlyingVector = position;
                particle.Velocity = velocity;
                particle.Size = 0.05f;
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

        private float Random(float min, float max)
        {
            return min + (max - min) * (float)random.Next(1000000) / 1000000.0f;
        }
    }
}
