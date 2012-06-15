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
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Missile(ContentManager content, Spaceship ship, Vector3 moveVector, Vector3 position)
        {
            model = XNAUtils.LoadModelWithBoundingSphere(ref transforms, ResourceNames.Missile, content);

            moveVector.Normalize();
            Vector2 ZX = new Vector2(moveVector.Z, moveVector.X);

            rotation = Quaternion.Identity;
            float horizontalAngle = (float)Math.Asin(moveVector.X / ZX.Length());
            if (moveVector.Z < 0)
                horizontalAngle = MathHelper.Pi - horizontalAngle;
            rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, horizontalAngle + MathHelper.PiOver2);
            float verticalAngle = (float)Math.Asin(moveVector.Y / moveVector.Length());
            rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, verticalAngle);
            this.scale = 0.1f;
            this.moveVector = moveVector;
            this.position = position;
        }

        public Missile(ContentManager content, Spaceship ship)
            : this(content, ship, Vector3.Transform(Vector3.Forward, ship.SpacecraftRotation), ship.SpacecraftPosition)
        {
            this.TimeToLive = 300;
        }

        public void Update(GameTime time, Vector3 centerOfUniverse)
        {
            float moveSpeed = 0.3f;
            rotation *= Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), 0.01f);

            position += moveSpeed * moveVector;
            position = ModelUtils.BendSpace(this, centerOfUniverse);
            TimeToLive -= 1;
        }

        public void Draw(ICamera fpsCam)
        {
            ModelUtils.Draw(this, fpsCam);
        }

        public Matrix GetJetOrientationMatrix()
        {
            var shift = new Vector3(0.0f, 0.0f, 0.5f);
            return Matrix.Multiply(WorldMatrix, Matrix.CreateTranslation(shift));
        }

        public int TimeToLive { get; set; }
    }
}
