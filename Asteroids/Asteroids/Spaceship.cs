﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    /// <summary>
    /// Klasa reprezentujaca sterowany przez gracza statek kosmiczny.
    /// </summary>
    public class Spaceship : IJet
    {
        Model model;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        private int lives = 3;

        public int Lives
        {
            get { return lives; }
        }

        Matrix[] transforms;
        Quaternion spacecraftRotation;
        Matrix worldMatrix;

        float updownRotation = 0.0f;
        float leftrightRotation = 0.0f;

        float velocity = 0;

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

        public Quaternion Rotation
        {
            get { return spacecraftRotation; }
            set { spacecraftRotation = value; }
        }
        Vector3 spacecraftPosition;

        public Vector3 Position
        {
            get { return spacecraftPosition; }
            set { spacecraftPosition = value; }
        }

        public Vector3 JetPosition
        {
            get
            {
                var shift = new Vector3(0.0f, 0.0f, 950.0f);
                return Vector3.Transform(shift, WorldMatrix);
            }
        }

        public Spaceship(ContentManager content)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, ResourceNames.Spaceship, content);
            spacecraftPosition = new Vector3(-1, 1, 5);
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
            float maxUpDownRotation = 0.03f;
            float upDownIncreaseStep = maxUpDownRotation / 30;
            float upDownDecreaseStep = maxUpDownRotation / 120;

            float maxLeftRightRotation = 0.05f;
            float leftRightIncreaseStep = maxUpDownRotation / 30;
            float leftRightDecreaseStep = maxUpDownRotation / 30;

            leftrightRotation -= gamePadState.ThumbSticks.Left.X / 50.0f;
            updownRotation += gamePadState.ThumbSticks.Left.Y / 50.0f;

            updownRotation = XNAUtils.CalculateVectorLengthWithInertia(keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S),
                keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W),
                updownRotation, upDownIncreaseStep, upDownDecreaseStep, maxUpDownRotation);

            leftrightRotation = XNAUtils.CalculateVectorLengthWithInertia(keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A),
                keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D),
                leftrightRotation, leftRightIncreaseStep, leftRightDecreaseStep, maxLeftRightRotation);


            if (keyboardState.IsKeyDown(Keys.RightShift) || keyboardState.IsKeyDown(Keys.LeftShift))
                Velocity = (Velocity >= 5) ? 5 : Velocity + 0.1f;
            else if (keyboardState.IsKeyDown(Keys.Z))
                Velocity = (Velocity <= 0.1) ? 0 : Velocity - 0.1f;
            else
                Velocity = (Velocity <= 0.3) ? 0.2f : Velocity - 0.02f;

            Quaternion additionalRotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), updownRotation) * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), leftrightRotation);
            Rotation = Rotation * additionalRotation;

            AddToSpacecraftPosition(new Vector3(updownRotation, 0, -Velocity));
        }

        public Matrix GetJetOrientationMatrix()
        {
            var shift = new Vector3(0.0f, 0.0f, 950.0f);
            return Matrix.Multiply(Matrix.CreateTranslation(shift), WorldMatrix);
        }

        private void AddToSpacecraftPosition(Vector3 vectorToAdd)
        {
            float moveSpeed = 0.05f;
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, Rotation);
            Position += moveSpeed * rotatedVector;
        }

        internal void Collide()
        {
            --lives;
        }
    }
}
