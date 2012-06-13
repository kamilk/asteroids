namespace Asteroids
{
    class TextureAndMaskNames
    {
        public string TextureName
        {
            get;
            private set;
        }

        public string MaskTextureName
        {
            get;
            private set;
        }

        public TextureAndMaskNames(string textureName, string maskTextureName)
        {
            this.TextureName = textureName;
            this.MaskTextureName = maskTextureName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TextureAndMaskNames;
            if (other == null)
                return false;
            return object.Equals(TextureName, other.TextureName)
                && object.Equals(MaskTextureName, other.MaskTextureName);
        }

        public override int GetHashCode()
        {
            return TextureName.GetHashCode() + MaskTextureName.GetHashCode();
        }
    }
}
