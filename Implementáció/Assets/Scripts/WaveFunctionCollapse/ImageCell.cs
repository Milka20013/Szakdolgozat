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
            var cellVariable = CollapseOptionManager.Instance.GetRandomCellVariable(collapseData.neighbours);
            if (cellVariable == null)
            {
                return null;
            }
            SetImage(cellVariable);
            collapseData.collapsed = true;

            return cellVariable;
        }
        public CellVariable CollapseCellWithLocalWeights()
        {

            var cellVariable = CollapseOptionManager.Instance.GetRandomCellVariableByLocalWeights(collapseData.neighbours);
            if (cellVariable == null)
            {
                return null;
            }
            SetImage(cellVariable);
            collapseData.collapsed = true;

            return cellVariable;
        }

        protected virtual void SetImage(CellVariable cellVariable)
        {
            image.sprite = cellVariable.sprite;
        }
        public void SetNeighbour(Side side, CellVariable cellVariable)
        {
            collapseData.SetNeighbour(side, cellVariable);
        }

        public virtual void Reset(Sprite sprite)
        {
            //image.sprite = sprite;
            collapseData = new();
        }
        public float Entropy()
        {
            return collapseData.Entropy();
        }
    }
}
