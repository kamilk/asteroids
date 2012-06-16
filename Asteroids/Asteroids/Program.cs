using System;

namespace Asteroids
{
#if WINDOWS || XBOX

    /// <summary>
    /// G³ówna klasa programu.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AsteroidsGame game = new AsteroidsGame())
            {
                game.Run();
            }
        }
    }
#endif
}

