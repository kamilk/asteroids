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
using System.Windows.Forms;

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
        List<Missile> missiles = new List<Missile>();
        Dictionary<Missile, MissileJetParticleEffect> missileEffectsByMissile = new Dictionary<Missile, MissileJetParticleEffect>();
        HashSet<MissileJetParticleEffect> missileEffects = new HashSet<MissileJetParticleEffect>();

        CoordCross coordCross;
        BasicEffect basicEffect;

        const int NUM_ASTEROIDS = 20;

        bool asteroidCollidesWithShip = false;
        int points = 0;

        SpriteFont spFont;
        SpriteBatch spBatch;

        ParticleSystem particleSystem;
        JetParticleEffect jetParticleEffect;

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

            skyboxModel = Content.Load<Model>(ResourceNames.SkyboxModel);
            skyboxTransforms = new Matrix[skyboxModel.Bones.Count];

            StartNewGame();
        }

        private void StartNewGame()
        {
            ship = new Spaceship(Content);
            camera = new SpaceshipCamera(graphics.GraphicsDevice.Viewport, ship);

            int x, y, z, pos_x, pos_y, pos_z;
            Random random = new Random();

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                x = random.Next(2);
                z = random.Next(2);
                y = random.Next(2);

                pos_x = random.Next(-30, 31);
                pos_y = random.Next(-30, 31);
                pos_z = random.Next(-30, 31);

                asteroids[i] = new Asteroid(Content, new Vector3(x, y, z));
                asteroids[i].Position = new Vector3(pos_x, pos_y, pos_z);
            }

            spriteDrawer = new SpriteDrawer(device, Content);
            spriteManager = new SpriteManager(device, Content);

            spFont = Content.Load<SpriteFont>(@"Arial");
            spBatch = new SpriteBatch(graphics.GraphicsDevice);

            particleSystem = new ParticleSystem(spriteManager);
            jetParticleEffect = new JetParticleEffect(particleSystem, ship);

            points = 0;
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

            FireMissileIfSpacePressed(keyboardState);

            // Check to see if the user has exited
            if (checkExitKey(keyboardState, gamePadState))
            {
                base.Update(gameTime);
                return;
            }

            UpdateShip(gameTime, ref gamePadState, ref keyboardState);
            UpdateCamera();
            UpdateAsteroids(gameTime);
            UpdateMissiles(gameTime);
            UpdateMissileEffects(gameTime);

            CollideAsteroidsWithShip();

            if (checkGameOver())
            {
                base.Update(gameTime);
                return;
            }

            CollideRocketsWithAsteroids();

            base.Update(gameTime);
        }

        private void FireMissileIfSpacePressed(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                if (spaceDown == false)
                {
                    spaceDown = true;
                    FireNewMissile();
                }
            }
            else
            {
                spaceDown = false;
            }
        }

        bool checkExitKey(KeyboardState keyboardState, GamePadState gamePadState)
        {
            // Check to see whether ESC was pressed on the keyboard 
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
                return true;
            }

            return false;
        }

        private bool checkGameOver()
        {
            if (asteroidCollidesWithShip)
            {
                ship.Collide();
                points -= 20;

                if (ship.Lives <= 0)
                {
                    if (MessageBox.Show("Straciłeś wszystkie życia. Grać ponownie?", "Koniec gry", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        StartNewGame();
                    }
                    else
                    {
                        Exit();
                        return true;
                    }
                }
            }
            return false;
        }

        private void UpdateShip(GameTime gameTime, ref GamePadState gamePadState, ref KeyboardState keyboardState)
        {
            ship.Update(Mouse.GetState(), keyboardState, gamePadState);
            jetParticleEffect.Update(gameTime);
        }

        private void UpdateCamera()
        {
            camera.Update();
        }

        private void UpdateAsteroids(GameTime gameTime)
        {
            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] != null)
                    asteroids[i].Update(gameTime, ship.SpacecraftPosition);
            }
        }

        private void UpdateMissiles(GameTime gameTime)
        {
            foreach (Missile missile in missiles)
            {
                missile.Update(gameTime, ship.SpacecraftPosition);
            }
            foreach (Missile missile in missiles.Where(m => m.TimeToLive < 0).ToArray())
            {
                DeleteMissile(missile);
            }
        }

        private void UpdateMissileEffects(GameTime gameTime)
        {
            var missileEffectsToRemove = new List<MissileJetParticleEffect>();
            foreach (var effect in missileEffects)
            {
                effect.Update(gameTime);
            }
            missileEffects.RemoveWhere(m => m.IsDead);
        }

        private void CollideAsteroidsWithShip()
        {
            asteroidCollidesWithShip = false;

            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] == null)
                    continue;

                if (XNAUtils.ModelsCollide(asteroids[i].Model, asteroids[i].WorldMatrix, ship.Model, ship.WorldMatrix))
                {
                    asteroidCollidesWithShip = true;
                    asteroids[i] = null;
                }
            }
        }

        private void CollideRocketsWithAsteroids()
        {
            // Check for collisions rocket - asteroid
            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                Asteroid asteroid = asteroids[i];
                if (asteroid == null)
                    continue;

                Missile missileToRemove = null;

                foreach (Missile missile in missiles)
                {
                    if (XNAUtils.ModelsCollide(asteroid.Model, asteroid.WorldMatrix, missile.Model, missile.WorldMatrix))
                    {
                        asteroids[i] = null;

                        // Store only the first one to collide, rest actually won't because asteroid no longer exists
                        // TODO: Some kind of explosion
                        missileToRemove = missile;
                        points += 10;
                        break;
                    }
                }

                if (missileToRemove != null)
                {
                    DeleteMissile(missileToRemove);
                }
            }
        }

        private void FireNewMissile()
        {
            Missile missile = new Missile(Content, ship);
            missiles.Add(missile);
            var effect = new MissileJetParticleEffect(particleSystem, missile);
            missileEffects.Add(effect);
            missileEffectsByMissile.Add(missile, effect);
        }

        private void DeleteMissile(Missile missile)
        {
            missiles.Remove(missile);
            MissileJetParticleEffect effect;
            if (missileEffectsByMissile.TryGetValue(missile, out effect))
            {
                effect.StopSpawningParticles();
                missileEffectsByMissile.Remove(missile);
            }
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

            spBatch.Begin();    //TODO: remove lives from text renderer and place 2d images representing lifes.
            spBatch.DrawString(spFont, String.Format("Ship: {0:f} {1:f} {2:f} | Lives: {3} | Points: {4:f}", ship.SpacecraftPosition.X,
                ship.SpacecraftPosition.Y, ship.SpacecraftPosition.Z, ship.Lives, points), new Vector2(10.0f, 10.0f), Color.White);
            spBatch.End();

            // TODO: remove test missile completely, probably test sprite in 0,0,0 too
            // no i uklad wspolrzednych tez
            //testMissile.Draw(camera);

            base.Draw(gameTime);
        }

    }
}
