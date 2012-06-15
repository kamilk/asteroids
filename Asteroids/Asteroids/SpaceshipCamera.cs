using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    class SpaceshipCamera : ICamera
    {
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Viewport viewPort;
        Spaceship ship;

        float leftrightRot;
        float updownRot;
        const float rotationSpeed = 0.005f;
        Vector3 cameraPosition;
        MouseState originalMouseState;


        public SpaceshipCamera(Viewport viewPort, Spaceship ship)
            : this(viewPort, ship, new Vector3(0, 1, 15), 0, 0)
        {
            //calls the constructor below with default startingPos and rotation values
        }

        public SpaceshipCamera(Viewport viewPort, Spaceship ship, Vector3 startingPos, float lrRot, float udRot)
        {
            this.ship = ship;
            this.leftrightRot = lrRot;
            this.updownRot = udRot;
            this.cameraPosition = startingPos;
            this.viewPort = viewPort;

            float viewAngle = MathHelper.PiOver4;
            float nearPlane = 0.1f;
            float farPlane = 100.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, viewPort.AspectRatio, nearPlane, farPlane);


            Mouse.SetPosition(viewPort.Width / 2, viewPort.Height / 2);
            originalMouseState = Mouse.GetState();
        }

        public void Update()
        {
            Vector3 cameraOriginalPosition = new Vector3(0.0f, 0.3f, 1.0f);
            Vector3 cameraRotatedPosition = Vector3.Transform(cameraOriginalPosition, ship.SpacecraftRotation);
            Vector3 cameraFinalPosition = ship.SpacecraftPosition + cameraRotatedPosition;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, ship.SpacecraftRotation);

            viewMatrix = Matrix.CreateLookAt(cameraFinalPosition, ship.SpacecraftPosition, cameraRotatedUpVector);
        }

        public float UpDownRot
        {
            get { return updownRot; }
            set { updownRot = value; }
        }

        public float LeftRightRot
        {
            get { return leftrightRot; }
            set { leftrightRot = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }
        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                Update();
            }
        }
        public Vector3 TargetPosition
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
                Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
                Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;
                return cameraFinalTarget;
            }
        }
        public Vector3 Forward
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraForward = new Vector3(0, 0, -1);
                Vector3 cameraRotatedForward = Vector3.Transform(cameraForward, cameraRotation);
                return cameraRotatedForward;
            }
        }
        public Vector3 SideVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalSide = new Vector3(1, 0, 0);
                Vector3 cameraRotatedSide = Vector3.Transform(cameraOriginalSide, cameraRotation);
                return cameraRotatedSide;
            }
        }
        public Vector3 UpVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Vector3 cameraOriginalUp = new Vector3(0, 1, 0);
                Vector3 cameraRotatedUp = Vector3.Transform(cameraOriginalUp, cameraRotation);
                return cameraRotatedUp;
            }
        }
    }
}

