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

        public Vector3 Position
        {
            get { return asteroidPosition; }
            set { asteroidPosition = value; }
        }

        public Asteroid(ContentManager content, float scale = 1.0f)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, "LargeAsteroid", content);
            asteroidPosition = new Vector3(0, 0, 0);
            rotation = Quaternion.Identity;
            this.scale = scale;
        }

        public void Update()
        {
            float moveSpeed = 0.05f;
            Vector3 moveVector = new Vector3(0, 0, 1);
            rotation *= Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.002f) * Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), 0.005f) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.003f);
            Position += moveSpeed * moveVector;
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
