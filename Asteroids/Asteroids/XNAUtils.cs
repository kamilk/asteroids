using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Asteroids
{
    /// <summary>
    /// Klasa zawierająca metody pomocnicze, przydatne przy korzystaniu z XNA.
    /// </summary>
    public static class XNAUtils
    {
        private static VertexDeclaration posColVertexDeclaration;
        public static BoundingBox TransformBoundingBox(BoundingBox origBox, Matrix matrix)
        {
            Vector3 origCorner1 = origBox.Min;
            Vector3 origCorner2 = origBox.Max;

            Vector3 transCorner1 = Vector3.Transform(origCorner1, matrix);
            Vector3 transCorner2 = Vector3.Transform(origCorner2, matrix);

            return new BoundingBox(transCorner1, transCorner2);
        }

        public static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
        {
            Vector3 trans;
            Vector3 scaling;
            Quaternion rot;
            transformationMatrix.Decompose(out scaling, out rot, out trans);

            float maxScale = scaling.X;
            if (maxScale < scaling.Y)
                maxScale = scaling.Y;
            if (maxScale < scaling.Z)
                maxScale = scaling.Z;

            float transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
            Vector3 transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

            BoundingSphere transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

            return transformedBoundingSphere;
        }

        public static Model LoadModelWithBoundingSphere(ref Matrix[] modelTransforms, string asset, ContentManager content)
        {
            Model newModel = content.Load<Model>(asset);

            modelTransforms = new Matrix[newModel.Bones.Count];
            newModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            BoundingSphere completeBoundingSphere = new BoundingSphere();
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                BoundingSphere origMeshSphere = mesh.BoundingSphere;
                BoundingSphere transMeshSphere = XNAUtils.TransformBoundingSphere(origMeshSphere, modelTransforms[mesh.ParentBone.Index]);
                completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
            }
            newModel.Tag = completeBoundingSphere;

            return newModel;
        }

        public static void DrawBoundingBox(BoundingBox bBox, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (posColVertexDeclaration == null)
                posColVertexDeclaration = new VertexDeclaration(VertexPositionColor.VertexDeclaration.GetVertexElements());

            Vector3 v1 = bBox.Min;
            Vector3 v2 = bBox.Max;

            VertexPositionColor[] cubeLineVertices = new VertexPositionColor[8];
            cubeLineVertices[0] = new VertexPositionColor(v1, Color.White);
            cubeLineVertices[1] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v1.Z), Color.Red);
            cubeLineVertices[2] = new VertexPositionColor(new Vector3(v2.X, v1.Y, v2.Z), Color.Green);
            cubeLineVertices[3] = new VertexPositionColor(new Vector3(v1.X, v1.Y, v2.Z), Color.Blue);

            cubeLineVertices[4] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v1.Z), Color.White);
            cubeLineVertices[5] = new VertexPositionColor(new Vector3(v2.X, v2.Y, v1.Z), Color.Red);
            cubeLineVertices[6] = new VertexPositionColor(v2, Color.Green);
            cubeLineVertices[7] = new VertexPositionColor(new Vector3(v1.X, v2.Y, v2.Z), Color.Blue);

            short[] cubeLineIndices = { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, cubeLineVertices, 0, 8, cubeLineIndices, 0, 12);
            }
        }

        public static void DrawSphereSpikes(BoundingSphere sphere, GraphicsDevice device, BasicEffect basicEffect, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (posColVertexDeclaration == null)
                posColVertexDeclaration = new VertexDeclaration(VertexPositionColor.VertexDeclaration.GetVertexElements());

            Vector3 up = sphere.Center + sphere.Radius * Vector3.Up;
            Vector3 down = sphere.Center + sphere.Radius * Vector3.Down;
            Vector3 right = sphere.Center + sphere.Radius * Vector3.Right;
            Vector3 left = sphere.Center + sphere.Radius * Vector3.Left;
            Vector3 forward = sphere.Center + sphere.Radius * Vector3.Forward;
            Vector3 back = sphere.Center + sphere.Radius * Vector3.Backward;

            VertexPositionColor[] sphereLineVertices = new VertexPositionColor[6];
            sphereLineVertices[0] = new VertexPositionColor(up, Color.White);
            sphereLineVertices[1] = new VertexPositionColor(down, Color.White);
            sphereLineVertices[2] = new VertexPositionColor(left, Color.White);
            sphereLineVertices[3] = new VertexPositionColor(right, Color.White);
            sphereLineVertices[4] = new VertexPositionColor(forward, Color.White);
            sphereLineVertices[5] = new VertexPositionColor(back, Color.White);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, sphereLineVertices, 0, 3);
            }
        }

        public static VertexPositionColor[] VerticesFromVector3List(List<Vector3> pointList, Color color)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[pointList.Count];

            int i = 0;
            foreach (Vector3 p in pointList)
                vertices[i++] = new VertexPositionColor(p, color);

            return vertices;
        }

        public static BoundingBox CreateBoxFromSphere(BoundingSphere sphere)
        {
            float radius = sphere.Radius;
            Vector3 outerPoint = new Vector3(radius, radius, radius);

            Vector3 p1 = sphere.Center + outerPoint;
            Vector3 p2 = sphere.Center - outerPoint;

            return new BoundingBox(p1, p2);
        }

        public static bool CoarseCheck(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            BoundingSphere origSphere1 = (BoundingSphere)model1.Tag;
            BoundingSphere sphere1 = XNAUtils.TransformBoundingSphere(origSphere1, world1);

            BoundingSphere origSphere2 = (BoundingSphere)model1.Tag;
            BoundingSphere sphere2 = XNAUtils.TransformBoundingSphere(origSphere1, world1);

            bool collision = sphere1.Intersects(sphere2);
            return collision;
        }

        public static bool ModelsCollide(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            if (CoarseCheck(model1, world1, model2, world2) == false)
                return false;

            Matrix[] model1Transforms = new Matrix[model1.Bones.Count];
            model1.CopyAbsoluteBoneTransformsTo(model1Transforms);
            BoundingSphere[] model1Spheres = new BoundingSphere[model1.Meshes.Count];
            for (int i = 0; i < model1.Meshes.Count; i++)
            {
                ModelMesh mesh = model1.Meshes[i];
                BoundingSphere origSphere = mesh.BoundingSphere;
                Matrix trans = model1Transforms[mesh.ParentBone.Index] * world1;
                BoundingSphere transSphere = XNAUtils.TransformBoundingSphere(origSphere, trans);
                model1Spheres[i] = transSphere;
            }
            Matrix[] model2Transforms = new Matrix[model2.Bones.Count];
            model2.CopyAbsoluteBoneTransformsTo(model2Transforms);
            BoundingSphere[] model2Spheres = new BoundingSphere[model2.Meshes.Count];
            for (int i = 0; i < model1.Meshes.Count; i++)
            {
                ModelMesh mesh = model2.Meshes[i];
                BoundingSphere origSphere = mesh.BoundingSphere;
                Matrix trans = model2Transforms[mesh.ParentBone.Index] * world2;
                BoundingSphere transSphere = XNAUtils.TransformBoundingSphere(origSphere, trans);
                model2Spheres[i] = transSphere;
            }
            bool collision = false;
            for (int i = 0; i < model1Spheres.Length; i++)
                for (int j = 0; j < model2Spheres.Length; j++)
                    if (model1Spheres[i].Intersects(model2Spheres[j]))
                        return true;
            return collision;
        }

        public static float CalculateVectorLengthWithInertia(bool increasing, bool decreasing, float currentValue, float increaseStep, float decreaseStep, float maxValue)
        {
            if (increasing)
                return (currentValue >= maxValue) ? maxValue : currentValue + increaseStep;
            else if (decreasing)
                return (currentValue <= -maxValue) ? -maxValue : currentValue - increaseStep;
            else
            {
                if (currentValue >= decreaseStep)
                {
                    return currentValue - decreaseStep;
                }
                else if (currentValue <= -decreaseStep)
                    return currentValue + decreaseStep;
                else
                    return 0;
            }
        }
    }
}
