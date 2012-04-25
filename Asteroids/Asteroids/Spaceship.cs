using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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
    }
}
