using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    public static class ModelUtils
    {
        public static void Draw(IModel model, ICamera fpsCam)
        {
            model.Model.CopyAbsoluteBoneTransformsTo(model.Transforms);
            model.WorldMatrix = Matrix.CreateScale(model.Scale) * Matrix.CreateFromQuaternion(model.Rotation)
                * Matrix.CreateTranslation(model.Position);

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = fpsCam.ViewMatrix;
                    effect.Projection = fpsCam.ProjectionMatrix;
                    effect.World = model.Transforms[mesh.ParentBone.Index] * model.WorldMatrix;
                }
                mesh.Draw();
            }
        }

        public static Vector3 BendSpace(IModel model)
        {
            float maxDistance = 70;
            Vector3 newPosition = model.Position;

            foreach (Vector3 direction in new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) })
            {
                Vector3 asteroidToShip = model.Position - model.Ship.SpacecraftPosition;
                if ((asteroidToShip * direction).Length() > maxDistance)
                {
                    Vector3 reverseDirection = model.Ship.SpacecraftPosition - ((asteroidToShip * direction).Length() * Vector3.One / asteroidToShip) * maxDistance;
                    newPosition = reverseDirection * direction + model.Position * (Vector3.One - direction);
                }
            }
            return newPosition;
        }
    }
}
