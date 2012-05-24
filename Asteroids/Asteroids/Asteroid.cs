using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Asteroid : IModel
    {
        private Model model;
        private Matrix[] transforms;
        private Quaternion rotation;
        private Matrix worldMatrix;
        private Vector3 asteroidPosition;
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

            asteroidPosition += moveSpeed * moveVector;
            asteroidPosition = ModelUtils.BendSpace(this);
        }

        public void Draw(ICamera fpsCam)
        {
            ModelUtils.Draw(this, fpsCam);
        }
    }
}
