using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public interface ICamera
    {
        Matrix ProjectionMatrix
        {
            get;
        }
        Matrix ViewMatrix
        {
            get;
        }
        Vector3 Position
        {
            get; set; 
        }
        void Update(MouseState currentMouseState, KeyboardState keyState, GamePadState gamePadState);
    }
}
