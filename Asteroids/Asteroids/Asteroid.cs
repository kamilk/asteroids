using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Asteroid
    {
        Model model;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        Matrix[] transforms;
        Quaternion rotation;
        Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        Vector3 asteroidPosition;
        private float scale;
        Spaceship ship;
        Vector3 moveVector;

        public Vector3 Position
        {
            get { return asteroidPosition; }
            set { asteroidPosition = value; }
        }

        public Asteroid(ContentManager content, Spaceship ship, Vector3 moveVector, float scale = 1.0f)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, "LargeAsteroid", content);
            asteroidPosition = new Vector3(0, 0, 0);
            rotation = Quaternion.Identity;
            this.ship = ship;
            this.scale = scale;
            this.moveVector = moveVector;
        }

        public void Update()
        {
            float moveSpeed = 0.05f;
            rotation *= Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.002f) * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), 0.005f) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.003f);

            float maxDistance = 70;

            foreach (Vector3 direction in new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) })
            {
                Vector3 asteroidToShip = asteroidPosition - ship.SpacecraftPosition;
                if (((asteroidToShip + moveSpeed * moveVector) * direction).Length() > maxDistance)
                {
                    Vector3 reverseDirection = ship.SpacecraftPosition - ((asteroidToShip * direction).Length() * Vector3.One / asteroidToShip) * maxDistance;
                    asteroidPosition = reverseDirection * direction + asteroidPosition * (Vector3.One - direction);
                }
            }
            asteroidPosition += moveSpeed * moveVector;
        }

        public void Draw(ICamera fpsCam)
        {
            model.CopyAbsoluteBoneTransformsTo(transforms);
            worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);

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
            Rotation = Rotation * additionalRotation;
        }
    }
}
