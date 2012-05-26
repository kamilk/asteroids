using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    class ParticleEffect
    {
        public ParticleSystem ParticleSystem
        {
            get;
            private set;
        }

        public ParticleEffect(ParticleSystem system)
        {
            this.ParticleSystem = system;
        }
    }
}
