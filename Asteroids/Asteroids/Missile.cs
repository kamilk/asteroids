using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    public class Missile : IModel
    {
        private Model model;
        private Matrix[] transforms;
        private Quaternion rotation;
        private Matrix worldMatrix;
        private Vector3 position;
        private float scale;
        private Spaceship ship;
        private Vector3 moveVector;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        public Matrix[] Transforms
        {
            get { return transforms; }
            set { transforms = value; }
        }
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
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public Spaceship Ship
        {
            get { return ship; }
            set { ship = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Missile(ContentManager content, Spaceship ship, Vector3 moveVector, Vector3 position)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, "missile", content);

            moveVector.Normalize();
            Vector2 ZX = new Vector2(moveVector.Z, moveVector.X);

            rotation = Quaternion.Identity;
            {
                float angle = (float)Math.Asin(moveVector.X / ZX.Length());
                if (moveVector.Z < 0)
                    angle = MathHelper.Pi - angle;
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, angle + MathHelper.PiOver2);
            }
            {
                float angle = (float)Math.Asin(moveVector.Y / moveVector.Length());
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, angle);
            }
            this.ship = ship;
            this.scale = 0.1f;
            this.moveVector = moveVector;
            this.position = position;
        }

        private Quaternion GetRotation(Vector3 src, Vector3 dest)
        {
            src.Normalize();
            dest.Normalize();

            float d = Vector3.Dot(src, dest);

            if (d >= 1f)
            {
                return Quaternion.Identity;
            }
            else if (d < (1e-6f - 1.0f))
            {
                Vector3 axis = Vector3.Cross(Vector3.UnitX, src);

                if (axis.LengthSquared() == 0)
                {
                    axis = Vector3.Cross(Vector3.UnitY, src);
                }

                axis.Normalize();
                return Quaternion.CreateFromAxisAngle(axis, MathHelper.Pi);
            }
            else
            {
                float s = (float)Math.Sqrt((1 + d) * 2);
                float invS = 1 / s;

                Vector3 c = Vector3.Cross(src, dest);
                Quaternion q = new Quaternion(invS * c, 0.5f * s);
                q.Normalize();

                return q;
            }
        }

        public void Update()
        {
            float moveSpeed = 0.0125f;
            //rotation *= Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.01f);

            position += moveSpeed * moveVector;
            position = ModelUtils.BendSpace(this);
        }

        public void Draw(ICamera fpsCam)
        {
            ModelUtils.Draw(this, fpsCam);
        }
    }
}
