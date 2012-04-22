using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Sphere
    {
        Model model;
        Matrix[] transforms;
        Quaternion sphereRotation;

        public Quaternion SphereRotation
        {
            get { return sphereRotation; }
            set { sphereRotation = value; }
        }
        Vector3 spherePosition;

        public Vector3 SpherePosition
        {
            get { return spherePosition; }
            set { spherePosition = value; }
        }

        public Sphere(ContentManager content)
        {
            model = content.Load<Model>("ship");
            transforms = new Matrix[model.Bones.Count];
            spherePosition = new Vector3(0, 0, 0);
            sphereRotation = Quaternion.Identity;
        }

        public void Draw(ICamera fpsCam)
        {
            model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix worldMatrix = Matrix.CreateScale(1.0f / 1000.0f) * Matrix.CreateFromQuaternion(sphereRotation) * Matrix.CreateTranslation(spherePosition);

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
            sphereRotation = sphereRotation * additionalRotation;
        }
    }
}
