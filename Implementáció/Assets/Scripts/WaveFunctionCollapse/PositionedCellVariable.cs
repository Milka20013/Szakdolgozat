using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class PositionedCellVariable
    {
        public PositionedCellVariable(CellVariable cellVariable, List<Vector2Int> positions)
        {
            this.cellVariable = cellVariable;
            this.positions = positions;
        }
        public CellVariable cellVariable;
        public List<Vector2Int> positions;
    }
}
