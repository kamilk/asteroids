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
        SpriteDrawer spriteDrawer;
        Texture2D spriteTexture;

        Model skyboxModel;
        Matrix[] skyboxTransforms;


        ICamera camera;

        Spaceship ship;
        Asteroid[] asteroids = new Asteroid[NUM_PLANETS];
        Sphere[] stars = new Sphere[NUM_STARS];

        CoordCross coordCross;
        BasicEffect basicEffect;

        const int NUM_PLANETS = 3;
        const int NUM_STARS = 2;
        private BatchOfSprites batchOfSprites;

        private Sprite sprite1;
        private Sprite sprite2;
        private Sprite sprite3;

        SpriteFont spFont;
        SpriteBatch spBatch;

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
            coordCross = new CoordCross(device);

            ship = new Spaceship(Content);
            camera = new SpaceshipCamera(graphics.GraphicsDevice.Viewport, ship);

            skyboxModel = Content.Load<Model>("skybox");
            skyboxTransforms = new Matrix[skyboxModel.Bones.Count];

            // Create a new SpriteBatch, which can be used to draw textures.

            asteroids[0] = new Asteroid(Content, ship, new Vector3(0, 0, 1));
            asteroids[1] = new Asteroid(Content, ship, new Vector3(0, 1, 1));
            asteroids[2] = new Asteroid(Content, ship, new Vector3(1, 1, 1));

            asteroids[0].Position = new Vector3(0, 0, -10);
            asteroids[1].Position = new Vector3(-10, 30, -20);
            asteroids[2].Position = new Vector3(30, -40, 35);

            stars[0] = new Sphere(Content);
            stars[1] = new Sphere(Content);

            stars[0].SpherePosition = new Vector3(0, 0, 0);
            stars[1].SpherePosition = new Vector3(40, 20, -20);

            spriteDrawer = new SpriteDrawer(device, Content);
            spriteTexture = Content.Load<Texture2D>("sprite");
            batchOfSprites = new BatchOfSprites(device);
            sprite1 = new Sprite(Vector3.Zero, 1.0f);
            sprite2 = new Sprite(new Vector3(3.0f, 4.0f, 1.0f), 2.0f, Color.Blue, 0.5f);
            sprite3 = new Sprite(new Vector3(-3.0f, 1.0f, 1.0f), 2.0f, Color.Green, -0.5f);
            batchOfSprites.AddSprite(sprite1);
            batchOfSprites.AddSprite(sprite2);
            batchOfSprites.AddSprite(sprite3);
            batchOfSprites.RemoveSprite(sprite3);

            spFont = Content.Load<SpriteFont>(@"Arial");
            spBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            spriteDrawer.Dispose();
            spriteDrawer = null;
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

            ship.Update(Mouse.GetState(), keyboardState, gamePadState);
            camera.Update();
            asteroids[0].Update();
            asteroids[1].Update();
            asteroids[2].Update();

            if (sprite2 != null)
            {
                float displacement = 3.0f * (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                sprite2.Position.X += displacement;
                if (sprite2.Position.X > 20.0f)
                {
                    batchOfSprites.RemoveSprite(sprite2);
                    sprite2 = null;
                    batchOfSprites.AddSprite(sprite3);
                }
            }

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
                    effect.LightingEnabled = false;

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(ship.SpacecraftPosition); ;
                }
                mesh.Draw();
            }
            device.DepthStencilState = DepthStencilState.Default;

            ship.Draw(camera);

            coordCross.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

            Matrix world1 = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-10, 0, 0);
            Matrix world2 = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(1000, 0, 0);

            bool anyColision = false;

            for (int i = 0; i < NUM_PLANETS; ++i)
            {
                asteroids[i].Draw(camera);
                if (XNAUtils.ModelsCollide(asteroids[i].Model, asteroids[i].WorldMatrix, ship.Model, ship.WorldMatrix))
                    anyColision = true;
            }

            for (int i = 0; i < NUM_STARS; ++i)
            {
                stars[i].Draw(camera);
                if (XNAUtils.ModelsCollide(stars[i].Model, stars[i].WorldMatrix, ship.Model, ship.WorldMatrix))
                    anyColision = true;
            }

            if (anyColision)
                Window.Title = "Asteroids - Kolizja";
            else
                Window.Title = "Asteroids";

            spriteDrawer.Begin(camera);
            spriteDrawer.SetTexture(spriteTexture);
            spriteDrawer.DrawBatchOfSprites(batchOfSprites);
            spriteDrawer.End();

            spBatch.Begin();
            spBatch.DrawString(spFont, String.Format("Ship: {0:f} {1:f} {2:f}", ship.SpacecraftPosition.X,
                ship.SpacecraftPosition.Y, ship.SpacecraftPosition.Z), new Vector2(10.0f, 10.0f), Color.White);
            spBatch.End();

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
