using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    public class ImageCell
    {

        public Image image;
        public CollapseData collapseData;

        public ImageCell(Image image)
        {
            this.image = image;
            this.collapseData = new();
        }
        public CellVariable CollapseCell()
        {
            var cellVariable = CollapseOptionManager.instance.GetRandomCellVariable(collapseData.neighbours);
            if (cellVariable == null)
            {
                return null;
            }
            image.sprite = cellVariable.sprite;
            collapseData.collapsed = true;

            return cellVariable;
        }
        public void SetNeighbour(Side side, CellVariable cellVariable)
        {
            collapseData.SetNeighbour(side, cellVariable);
        }

        public void Reset(Sprite sprite)
        {
            image.sprite = sprite;
            collapseData = new();
        }
        public float Entropy()
        {
            return collapseData.Entropy();
        }
    }
}
