using System;
using Microsoft.Xna.Framework;

namespace Asteroids
{
    /// <summary>
    /// Efekt cząsteczkowy silnika odrzutowego.
    /// </summary>
    class JetParticleEffect : ParticleEffect
    {
        private double nextSpawnTime;
        private IJet jet;

        private bool active = true;

        public float MinXDirection
        { get; set; }

        public float MaxXDirection
        { get; set; }

        public float MinZDirection
        { get; set; }

        public float MaxZDirection
        { get; set; }

        public float MinYDirection
        { get; set; }

        public float MaxYDirection
        { get; set; }

        public float ParticleSpeed
        { get; set; }

        public float ParticleSize
        { get; set; }

        public double ParticlesPerSecond
        { get; set; }

        public double ParticleLifetime
        { get; set; }

        public Color InitialColor
        { get; set; }

        public Color FinalColor
        { get; set; }
        
        public bool IsDead
        {
            get { return !active && !HasAnyParticles; }
        }

        public JetParticleEffect(ParticleSystem system, IJet jet)
            : base(system)
        {
            this.jet = jet;

            this.MinXDirection = 0.6f;
            this.MaxXDirection = 0.8f;

            this.MinZDirection = -0.2f;
            this.MaxZDirection = 0.2f;

            this.MinYDirection = -0.2f;
            this.MaxYDirection = 0.2f;

            this.ParticleSpeed = 0.5f;
            this.ParticleSize = 0.05f;

            this.ParticlesPerSecond = 30.0;
            this.ParticleLifetime = 2000.0;

            this.InitialColor = this.FinalColor = Color.White;
        }

        public void StopSpawningParticles()
        {
            active = false;
        }

        protected override void OnBeforeFirstUpdate(GameTime time)
        {
            nextSpawnTime = time.TotalGameTime.TotalMilliseconds + 1000.0f / ParticlesPerSecond;
        }

        protected override void UpdateEffect(GameTime time)
        {
            if (!active)
                return;

            if (!ShouldNewParticleSpawn(time))
                return;

            do
            {
                nextSpawnTime += 1000.0f / ParticlesPerSecond;

                Vector3 jetDirection = new Vector3(
                    AsteroidsUtilities.Random(MinXDirection, MaxXDirection),
                    AsteroidsUtilities.Random(MinYDirection, MaxYDirection),
                    AsteroidsUtilities.Random(MinZDirection, MaxZDirection));

                Vector3 velocity = Vector3.Transform(jetDirection, jet.Rotation);
                velocity.Normalize();
                velocity *= ParticleSpeed;

                var particle = CreateParticle(ResourceNames.ParticleTexture, ResourceNames.ParticleMask, time);
                particle.Position.UnderlyingVector = jet.JetPosition;
                particle.Velocity = velocity;
                particle.Size = ParticleSize;
            } while (ShouldNewParticleSpawn(time));
        }

        protected override bool UpdateParticle(Particle particle, GameTime time)
        {
            if (particle.SpawnTime.TotalMilliseconds + ParticleLifetime < time.TotalGameTime.TotalMilliseconds)
                return false;

            particle.Position.UnderlyingVector += particle.Velocity * (float)time.ElapsedGameTime.TotalSeconds;

            if (InitialColor != FinalColor)
            {
                double fractionOfLifeLived = (double)(time.TotalGameTime - particle.SpawnTime).TotalMilliseconds / ParticleLifetime;
                Color color = new Color();
                color.R = Interpolate(fractionOfLifeLived, InitialColor.R, FinalColor.R);
                color.G = Interpolate(fractionOfLifeLived, InitialColor.G, FinalColor.G);
                color.B = Interpolate(fractionOfLifeLived, InitialColor.B, FinalColor.B);
                color.A = Interpolate(fractionOfLifeLived, InitialColor.A, FinalColor.A);
                particle.Color = color;
            }

            return true;
        }

        private static byte Interpolate(double fraction, byte initial, byte final)
        {
            return (byte)(initial + (byte)(fraction * (double)(final - initial)));
        }

        private bool ShouldNewParticleSpawn(GameTime time)
        {
            return nextSpawnTime < time.TotalGameTime.TotalMilliseconds;
        }
    }
}
