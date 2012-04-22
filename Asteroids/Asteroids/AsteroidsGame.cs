using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AsteroidsGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;

        Model skyboxModel;
        Matrix[] skyboxTransforms;

        ICamera fpsCam;

        Spaceship ship;
        Sphere[] planets = new Sphere[NUM_PLANETS];
        Sphere[] stars = new Sphere[NUM_STARS];

        CoordCross cCross;
        BasicEffect basicEffect;

        const int NUM_PLANETS = 3;
        const int NUM_STARS = 2;

        public AsteroidsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = samplerState;

            basicEffect = new BasicEffect(device);
            cCross = new CoordCross(device);

            ship = new Spaceship(Content);
            fpsCam = new SpaceshipCamera(graphics.GraphicsDevice.Viewport, ship);

            skyboxModel = Content.Load<Model>("skybox");
            skyboxTransforms = new Matrix[skyboxModel.Bones.Count];

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            planets[0] = new Sphere(Content);
            planets[1] = new Sphere(Content);
            planets[2] = new Sphere(Content);

            planets[0].SpherePosition = new Vector3(10, 10, 10);
            planets[1].SpherePosition = new Vector3(-10, 30, -20);
            planets[2].SpherePosition = new Vector3(100, -40, 70);

            stars[0] = new Sphere(Content);
            stars[1] = new Sphere(Content);

            stars[0].SpherePosition = new Vector3(0, 0, 0);
            stars[1].SpherePosition = new Vector3(40, 20, -20);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Check to see if the user has exited
            if (checkExitKey(keyboardState, gamePadState))
            {
                base.Update(gameTime);
                return;
            }

            fpsCam.Update(Mouse.GetState(), keyboardState, gamePadState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            device.DepthStencilState = DepthStencilState.DepthRead;
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);

            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = fpsCam.ViewMatrix;
                    effect.Projection = fpsCam.ProjectionMatrix;
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(ship.SpacecraftPosition); ;
                }
                mesh.Draw();
            }
            device.DepthStencilState = DepthStencilState.Default;

            ship.Draw(fpsCam);

            cCross.Draw(fpsCam.ViewMatrix, fpsCam.ProjectionMatrix);

            for (int i = 0; i < NUM_PLANETS; ++i)
            {
                planets[i].Draw(fpsCam);
            }

            for (int i = 0; i < NUM_STARS; ++i)
            {
                stars[i].Draw(fpsCam);
            }

            base.Draw(gameTime);
        }

        bool checkExitKey(KeyboardState keyboardState, GamePadState gamePadState)
        {
            // Check to see whether ESC was pressed on the keyboard 
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
                return true;
            }

            return false;
        }
    }
}