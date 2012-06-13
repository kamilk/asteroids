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
        SpriteManager spriteManager;

        Model skyboxModel;
        Matrix[] skyboxTransforms;

        SpaceshipCamera camera;

        Spaceship ship;
        Asteroid[] asteroids = new Asteroid[NUM_ASTEROIDS];
        IList<Missile> missiles = new List<Missile>();

        CoordCross coordCross;
        BasicEffect basicEffect;

        const int NUM_ASTEROIDS = 20;

        bool collided_object_ship = false;
        bool has_just_collided = false;
        int points = 0;

        private Sprite sprite1;
        private Sprite sprite2;
        private Sprite sprite3;

        SpriteFont spFont;
        SpriteBatch spBatch;

        ParticleSystem particleSystem;
        JetParticleEffect jetParticleEffect;

        Missile testMissile;

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

            int x, y, z, pos_x, pos_y, pos_z;
            Random random = new Random();

            for(int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                x = random.Next(2);
                z = random.Next(2);
                y = random.Next(2);

                pos_x = random.Next(-30, 31);
                pos_y = random.Next(-30, 31);
                pos_z = random.Next(-30, 31);

                asteroids[i] = new Asteroid(Content, ship, new Vector3(x, y, z));
                asteroids[i].Position = new Vector3(pos_x, pos_y, pos_z);
            }

            spriteDrawer = new SpriteDrawer(device, Content);
            spriteManager = new SpriteManager(device, Content);
            
            sprite1 = spriteManager.CreateSprite("sprite");
            sprite1.Position.UnderlyingVector = Vector3.Zero;
            sprite1.Size = 0.1f;

            sprite2 = spriteManager.CreateSprite("sprite");
            sprite2.Position.UnderlyingVector = new Vector3(3.0f, 4.0f, 1.0f);
            sprite2.Size = 0.5f;
            sprite2.Color = Color.Blue;

            sprite3 = spriteManager.CreateSprite("sprite");
            sprite3.Position.UnderlyingVector = new Vector3(-3.0f, 1.0f, 1.0f);
            sprite3.Size = 0.03f;
            sprite3.Color = Color.Green;

            spriteManager.DeleteSprite(sprite3);

            spFont = Content.Load<SpriteFont>(@"Arial");
            spBatch = new SpriteBatch(graphics.GraphicsDevice);

            particleSystem = new ParticleSystem(spriteManager);
            jetParticleEffect = new JetParticleEffect(particleSystem, ship);

            testMissile = new Missile(Content, ship, particleSystem);
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

        private bool spaceDown = false;

        protected override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (spaceDown == false)
                {
                    spaceDown = true;
                    missiles.Add(new Missile(Content, ship, particleSystem));
                }
            }
            else
            {
                spaceDown = false;
            }

            // Check to see if the user has exited
            if (checkExitKey(keyboardState, gamePadState))
            {
                base.Update(gameTime);
                return;
            }

            ship.Update(Mouse.GetState(), keyboardState, gamePadState);
            camera.Update();

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] == null)
                    continue;

                asteroids[i].Update(gameTime);
            }


            foreach(Missile missile in missiles)
            {
                missile.Update(gameTime);
            }

            if (sprite2 != null)
            {
                float displacement = 3.0f * (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                sprite2.Position.X += displacement;
                if (sprite2.Position.X > 20.0f)
                {
                    spriteManager.DeleteSprite(sprite2);
                    sprite2 = null;

                    sprite3 = spriteManager.CreateSprite("sprite");
                    sprite3.Position.UnderlyingVector = new Vector3(-3.0f, 1.0f, 1.0f);
                    sprite3.Size = 0.05f;
                    sprite3.Color = Color.Green;
                }
            }

            testMissile.Position = new Vector3(0.0f, 1.0f, 1.0f);

            if (sprite1 != null && testMissile != null)
            {
                sprite1.Position.UnderlyingVector = Vector3.Transform(Vector3.Zero, testMissile.GetJetOrientationMatrix());
            }

            if (sprite3 != null)
            {
                sprite3.Position.UnderlyingVector = Vector3.Transform(Vector3.Zero, ship.GetJetOrientationMatrix());
            }

            jetParticleEffect.Update(gameTime);

            collided_object_ship = false;

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] == null)
                    continue;

                if (XNAUtils.ModelsCollide(asteroids[i].Model, asteroids[i].WorldMatrix, ship.Model, ship.WorldMatrix))
                {
                    collided_object_ship = true;
                    asteroids[i] = null;
                }
            }

            if (collided_object_ship)
            {
                if (!has_just_collided)

                    if (ship.Collide_DoesEnd())
                        Exit();     //TODO: some end game text, option to try again etc. (possibly)

                has_just_collided = true;
                points -= 20;
            }
            else
            {
                has_just_collided = false;
            }

            int index = -1;

            foreach (Missile missile in missiles)
            {
                // Check for collisions rocket - asteroid
                for (int i = 0; i < NUM_ASTEROIDS; ++i)
                {
                    if (asteroids[i] == null)
                        continue;

                    if (XNAUtils.ModelsCollide(asteroids[i].Model, asteroids[i].WorldMatrix, missile.Model, missile.WorldMatrix))
                    {
                        asteroids[i] = null;

                        // Store only the first one to collide, rest actually won't because asteroid no longer exists
                        // TODO: Some kind of explosion
                        if (index < 0)
                            index = missiles.IndexOf(missile);

                        points += 10;
                    }
                }
            }

            // Has to be done after the loop
            if (index >= 0)
                missiles.RemoveAt(index);

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

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] == null)
                    continue;

                asteroids[i].Draw(camera);
            }

            foreach (Missile missile in missiles)
            {
                missile.Draw(camera);
            }

            spriteManager.DrawAll(spriteDrawer, camera);

            spBatch.Begin();    //TODO: remove lifes from text renderer and place 2d images representing lifes.
            spBatch.DrawString(spFont, String.Format("Ship: {0:f} {1:f} {2:f} | Lifes: {3:f} | Points: {4:f}", ship.SpacecraftPosition.X,
                ship.SpacecraftPosition.Y, ship.SpacecraftPosition.Z, ship.lifes, points), new Vector2(10.0f, 10.0f), Color.White);
            spBatch.End();

            // TODO: remove test missile completely, probably test sprite in 0,0,0 too
            // no i uklad wspolrzednych tez
            //testMissile.Draw(camera);

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
