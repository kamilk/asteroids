using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace Asteroids
{
    /// <summary>
    /// Klasa opakowująca Vector3, która może powiadamiać o zmianie wartości wektora.
    /// </summary>
    public class ObservableVector3
    {
        private Vector3 vector;

        public float X
        {
            get
            {
                return vector.X;
            }
            set
            {
                vector.X = value;
                NotifyChanged();
            }
        }

        public float Y
        {
            get
            {
                return vector.Y;
            }
            set
            {
                vector.Y = value;
                NotifyChanged();
            }
        }

        public float Z
        {
            get
            {
                return vector.Z;
            }
            set
            {
                vector.Z = value;
                NotifyChanged();
            }
        }

        public Vector3 UnderlyingVector
        {
            get { return vector; }
            set
            {
                vector = value;
                NotifyChanged();
            }
        }

        public event EventHandler Changed;

        public ObservableVector3(Vector3 vector)
        {
            this.vector = vector;
        }

        public void Set(ObservableVector3 value)
        {
            UnderlyingVector = value.UnderlyingVector;
        }

        private void NotifyChanged()
        {
            if (Changed != null)
                Changed(this, new EventArgs());
        }
    }
}
