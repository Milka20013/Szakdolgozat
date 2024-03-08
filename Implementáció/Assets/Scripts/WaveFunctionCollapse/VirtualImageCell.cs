using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    public class VirtualImageCell : ImageCell
    {
        private Vector2Int position;
        private Texture2D texture;
        public VirtualImageCell(Image image, Vector2Int position) : base(image)
        {
            this.image = image;
            texture = image.sprite.texture;
            this.position = position;
            collapseData = new();
        }

        protected override void SetImage(CellVariable cellVariable)
        {
            var width = cellVariable.sprite.texture.width;
            var height = cellVariable.sprite.texture.height;
            var pixels = cellVariable.sprite.texture.GetPixels();
            texture.SetPixels(position.x * width, position.y * height, width, height, pixels);
            //texture.Apply();
        }
        public override void Reset(Sprite sprite)
        {
            this.collapseData = new();
        }
    }
}
