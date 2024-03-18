using System.Collections.Generic;

namespace WFC
{
    /// <summary>
    /// Helper class inside the cells. Helps with entropy and neighboring.
    /// </summary>
    public class CollapseData
    {
        public CollapseData()
        {
            neighbours = new();
            collapsed = false;
        }
        public Dictionary<Side, CellVariable> neighbours;
        public bool collapsed;
        public float Entropy()
        {
            if (collapsed)
            {
                return float.PositiveInfinity;
            }
            return CollapseOptionManager.Instance.Entropy(neighbours);
        }
        public void SetNeighbour(Side side, CellVariable cellVariable)
        {
            neighbours.TryAdd(side, cellVariable);
        }
    }
}
