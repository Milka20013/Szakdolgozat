using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    /// <summary>
    /// Physical representation of a cellVariable
    /// </summary>
    public class ImageCell
    {
        public string name;
        public Image image;
        public CollapseData collapseData;

        public ImageCell(Image image)
        {
            this.image = image;
            this.collapseData = new();
        }
        /// <summary>
        /// Determine the state of the cell and set their data onto the image based on global weights
        /// </summary>
        /// <returns></returns>
        public CellVariable CollapseCell()
        {
            var cellVariable = CollapseOptionManager.Instance.GetRandomCellVariable(collapseData.neighbours);
            if (cellVariable == null)
            {
                return null;
            }
            name = cellVariable.name;
            SetImage(cellVariable);
            collapseData.collapsed = true;

            return cellVariable;
        }
        /// <summary>
        /// Determine the state of the cell and set their data onto the image based on local weights
        /// </summary>
        /// <returns></returns>
        public CellVariable CollapseCellWithLocalWeights()
        {

            var cellVariable = CollapseOptionManager.Instance.GetRandomCellVariableByLocalWeights(collapseData.neighbours);
            if (cellVariable == null)
            {
                return null;
            }
            name = cellVariable.name;
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
