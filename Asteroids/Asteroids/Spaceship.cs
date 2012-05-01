using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public class Spaceship
    {
        Model model;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        Matrix[] transforms;
        Quaternion spacecraftRotation;
        Matrix worldMatrix;

        float velocity;

        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }


        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public Quaternion SpacecraftRotation
        {
            get { return spacecraftRotation; }
            set { spacecraftRotation = value; }
        }
        Vector3 spacecraftPosition;

        public Vector3 SpacecraftPosition
        {
            get { return spacecraftPosition; }
            set { spacecraftPosition = value; }
        }

        public Spaceship(ContentManager content)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, "ship", content);
            spacecraftPosition = new Vector3(-1, 1, 10);
            spacecraftRotation = Quaternion.Identity;
            velocity = 0;
        }

        public void Draw(ICamera fpsCam)
        {
            model.CopyAbsoluteBoneTransformsTo(transforms);
            worldMatrix = Matrix.CreateScale(1.0f / 5000.0f) * Matrix.CreateFromQuaternion(spacecraftRotation) * Matrix.CreateTranslation(spacecraftPosition);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = fpsCam.ViewMatrix;
                    effect.Projection = fpsCam.ProjectionMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * worldMatrix;
                }
                mesh.Draw();
            }
        }

        public void AddRotation(Quaternion additionalRotation)
        {
            spacecraftRotation = spacecraftRotation * additionalRotation;
        }

        public void Update(MouseState currentMouseState, KeyboardState keyboardState, GamePadState gamePadState)
        {
            float updownRotation = 0.0f;
            float leftrightRotation = 0.0f;
            float speed = 0.0f;

            leftrightRotation -= gamePadState.ThumbSticks.Left.X / 50.0f;
            updownRotation += gamePadState.ThumbSticks.Left.Y / 50.0f;

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                updownRotation = -0.05f;
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                updownRotation = 0.05f;
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                leftrightRotation = -0.05f;
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                leftrightRotation = 0.05f;
            if (keyboardState.IsKeyDown(Keys.RightShift) || keyboardState.IsKeyDown(Keys.LeftShift))
            {
                Velocity = (Velocity >= 5) ? 5 : Velocity + 0.1f;
            }
            else if (keyboardState.IsKeyDown(Keys.Z))
            {
                Velocity = (Velocity <= 0.1) ? 0 : Velocity - 0.1f;
            }
            else
            {
                Velocity = (Velocity <= 0.3) ? 0.2f : Velocity - 0.02f;
            }


            leftrightRotation -= gamePadState.ThumbSticks.Left.X / 50.0f;
            updownRotation += gamePadState.ThumbSticks.Left.Y / 50.0f;

            Quaternion additionalRotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), updownRotation) * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), leftrightRotation);
            SpacecraftRotation = SpacecraftRotation * additionalRotation;

            AddToSpacecraftPosition(new Vector3(0, 0, -Velocity));
        }

        private void AddToSpacecraftPosition(Vector3 vectorToAdd)
        {
            float moveSpeed = 0.05f;
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, SpacecraftRotation);
            SpacecraftPosition += moveSpeed * rotatedVector;
        }
    }
}
