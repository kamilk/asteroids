using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace Asteroids
{
    class Sprite : INotifyPropertyChanged
    {
        int hash;

        private ObservableVector3 _Position;
        public ObservableVector3 Position
        {
            get
            {
                return _Position;
            }
        }

        private Color _Color;
        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
                NotifyPropertyChanged("Color");
            }
        }
        private float _Size;
        public float Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                NotifyPropertyChanged("Size");
            }
        }
        private float _Rotation;
        public float Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                _Rotation = value;
                NotifyPropertyChanged("Rotation");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Sprite(Vector3 position, float size, Color color, float rotation = 0.0f)
        {
            this._Position = new ObservableVector3(position);
            this.Color = color;
            this.Size = size;
            this.Rotation = rotation;

            hash = new Random().Next();

            _Position.PropertyChanged += new PropertyChangedEventHandler(OnPositionChanged);
        }

        public Sprite(Vector3 position, float size)
            : this(position, size, Color.White)
        { }

        public override int GetHashCode()
        {
            return hash;
        }

        private void OnPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("Position");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
