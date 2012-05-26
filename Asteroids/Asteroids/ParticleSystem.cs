using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class ParticleSystem
    {
        private SpriteManager spriteManager;

        public ParticleSystem(SpriteManager spriteManager)
        {
            this.spriteManager = spriteManager;
        }
    }
}
