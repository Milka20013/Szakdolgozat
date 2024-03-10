using ProjectCore;
using System.Collections.Generic;
using System.Linq;

namespace WFC
{
    public class LocalWeightConnection : IWeighted
    {
        public static List<LocalWeightConnection> Connections { get; private set; } = new();
        public (CellVariable, CellVariable) cells;
        public float weight;

        private LocalWeightConnection(CellVariable c1, CellVariable c2)
        {
            cells = (c1, c2);
            weight = 1;
        }

        public static void CreateOrRegister(CellVariable c1, CellVariable c2)
        {
            if (c1 == null || c2 == null)
            {
                return;
            }
            var existingConnection = Connections.Where(x => x.Exists(c1, c2)).FirstOrDefault();
            if (existingConnection != null)
            {
                existingConnection.weight++;
                return;
            }
            Connections.Add(new(c1, c2));
        }

        public static float GetWeightOfConnection(CellVariable c1, CellVariable c2)
        {
            var existingConnection = Connections.Where(x => x.Exists(c1, c2)).FirstOrDefault();
            if (existingConnection != null)
            {
                return existingConnection.weight;
            }
            return -1;
        }
        public static CellVariable GetRandomCellVariableFromWeightedConnections(CellVariable c1)
        {
            var connections = Connections.Where(x => x.Has(c1));
            var chosenConnection = ProjectManager.RandomElementWeighted(connections);
            return chosenConnection.GetPair(c1);
        }

        public CellVariable GetPair(CellVariable c1)
        {
            if (cells.Item1 == c1)
            {
                return cells.Item2;
            }
            if (cells.Item2 == c1)
            {
                return cells.Item1;
            }
            return null;
        }
        public bool Has(CellVariable cell)
        {
            if (cells.Item1 == cell || cells.Item2 == cell)
            {
                return true;
            }
            return false;
        }
        public bool Exists(CellVariable cell1, CellVariable cell2)
        {
            if (cells.Item1 == cell1)
            {
                if (cells.Item2 == cell2)
                {
                    return true;
                }
            }
            else if (cells.Item1 == cell2)
            {
                if (cells.Item2 == cell1)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return cells.Item1.name + "  " + cells.Item2.name + "  " + weight;
        }

        public float GetWeight()
        {
            return weight;
        }
    }
}
