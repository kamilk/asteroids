using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace Asteroids
{
    class ObservableVector3 : INotifyPropertyChanged
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
                NotifyPropertyChanged("X");
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
                NotifyPropertyChanged("Y");
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
                NotifyPropertyChanged("Z");
            }
        }

        public ObservableVector3(Vector3 vector)
        {
            this.vector = vector;
        }

        public Vector3 GetVector3()
        {
            return vector;
        }

        private void NotifyPropertyChanged(string paramName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(paramName));
        }
    
        public event PropertyChangedEventHandler  PropertyChanged;
    }
}
