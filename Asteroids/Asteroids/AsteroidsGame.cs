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
    /// Główna klasa gry
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
        Dictionary<Missile, JetParticleEffect> missileEffectsByMissile = new Dictionary<Missile, JetParticleEffect>();
        HashSet<JetParticleEffect> missileEffects = new HashSet<JetParticleEffect>();

        CoordCross coordCross;
        BasicEffect basicEffect;

        const int NUM_ASTEROIDS = 20;

        bool collided_object_ship = false;
        bool has_just_collided = false;
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
            jetParticleEffect.MinXDirection = -0.3f;
            jetParticleEffect.MaxXDirection = 0.3f;
            jetParticleEffect.MinZDirection = 0.6f;
            jetParticleEffect.MaxZDirection = 0.8f;
            jetParticleEffect.MinYDirection = -0.2f;
            jetParticleEffect.MaxYDirection = 0.2f;
            jetParticleEffect.ParticlesPerSecond = 50.0;
            jetParticleEffect.ParticleLifetime = 400.0;
            jetParticleEffect.ParticleSize = 0.5f;
            jetParticleEffect.ParticleSpeed = 0.7f;
            jetParticleEffect.FinalColor = Color.Red;
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
            spriteManager.Dispose();
            spriteManager = null;
        }

        private bool spaceDown = false;

        protected override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

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

                asteroids[i].Update(gameTime, ship.Position);
            }

            foreach (Missile missile in missiles)
            {
                missile.Update(gameTime, ship.Position);
            }
            foreach (Missile missile in missiles.Where(m => m.HasDied).ToArray())
            {
                DeleteMissile(missile);
            }

            jetParticleEffect.Update(gameTime);

            UpdateMissileEffects(gameTime);

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
                    ship.Collide();
                    if (ship.Lives <= 0)
                        if (MessageBox.Show("Straciłeś wszystkie życia. Grać ponownie?", "Koniec gry", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            StartNewGame();
                            return;
                        }
                        else
                        {
                            Exit();
                        }

                has_just_collided = true;
                points -= 20;
            }
            else
            {
                has_just_collided = false;
            }

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

            bool shouldEnd = true;

            // Check if the game should end
            for (int i = 0; i < NUM_ASTEROIDS; ++i)
            {
                if (asteroids[i] == null)
                    continue;
                else
                {
                    shouldEnd = false;
                    break;
                }
            }

            if (shouldEnd)
                Exit(); 

            base.Update(gameTime);
        }

        private void UpdateMissileEffects(GameTime gameTime)
        {
            var missileEffectsToRemove = new List<JetParticleEffect>();
            foreach (var effect in missileEffects)
            {
                effect.Update(gameTime);
            }
            missileEffects.RemoveWhere(m => m.IsDead);
        }

        private void FireNewMissile()
        {
            Missile missile = new Missile(Content, ship);
            missiles.Add(missile);

            var effect = new JetParticleEffect(particleSystem, missile);
            effect.MinXDirection = 0.6f;
            effect.MaxXDirection = 0.8f;
            effect.MinZDirection = -0.025f;
            effect.MaxZDirection = 0.025f;
            effect.MinYDirection = -0.025f;
            effect.MaxYDirection = 0.025f;
            effect.ParticlesPerSecond = 30.0;
            effect.ParticleLifetime = 2000.0;
            effect.ParticleSize = 0.2f;
            effect.ParticleSpeed = 5.0f;

            missileEffects.Add(effect);
            missileEffectsByMissile.Add(missile, effect);
        }

        private void DeleteMissile(Missile missile)
        {
            missiles.Remove(missile);
            JetParticleEffect effect;
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
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(ship.Position); ;
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
            spBatch.DrawString(spFont, String.Format("Ship: {0:f} {1:f} {2:f} | Lives: {3} | Points: {4:f}", ship.Position.X,
                ship.Position.Y, ship.Position.Z, ship.Lives, points), new Vector2(10.0f, 10.0f), Color.White);
            spBatch.End();

            // TODO: remove test missile completely, probably test sprite in 0,0,0 too
            // no i uklad wspolrzednych tez
            //testMissile.Draw(camera);

            base.Draw(gameTime);
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
    }
}
