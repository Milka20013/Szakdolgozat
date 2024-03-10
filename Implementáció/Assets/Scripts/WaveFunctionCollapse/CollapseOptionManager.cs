using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC
{
    public class CollapseOptionManager : MonoBehaviour
    {
        public static CollapseOptionManager Instance { get; private set; }
        public CellVariable[] cellVariables;
        public CellVariable wildCard;
        public float bias = 1.5f;

        private void Awake()
        {
            Instance = this;
        }

        public float Entropy(Dictionary<Side, CellVariable> neighbours)
        {
            var options = GetPossibleCellVariables(neighbours);
            float sumWeight = 0;
            float[] weights = new float[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                weights[i] = options.ElementAt(i).weight;
                sumWeight += weights[i];
            }
            float entropy = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                entropy += weights[i] / sumWeight * Mathf.Log(sumWeight / weights[i]);
            }
            return entropy;
        }
        public CellVariable GetRandomCellVariable(Dictionary<Side, CellVariable> neighbours)
        {
            HashSet<CellVariable> options = GetPossibleCellVariables(neighbours);
            if (options.Count == 0)
            {
                if (wildCard != null)
                {
                    return wildCard;
                }
                return null;
            }
            return GameManager.RandomElementWeighted(options);
        }

        public CellVariable GetRandomCellVariableByLocalWeights(Dictionary<Side, CellVariable> neighbours)
        {
            HashSet<CellVariable> options = GetPossibleCellVariables(neighbours);
            if (options.Count == 0)
            {
                if (wildCard != null)
                {
                    return wildCard;
                }
                return null;
            }
            List<CellVariable> candidates = new(options);
            foreach (var neighbor in neighbours.Values.ToArray())
            {
                candidates.Add(neighbor.PreferredOption(options.ToList()));
            }
            candidates.RemoveAll(x => x == null);
            if (!candidates.Any())
            {
                if (wildCard != null)
                {
                    return wildCard;
                }
                return null;
            }
            var weightedCandidatesImmutable = candidates.GroupBy(x => x).Select(x => new { element = x.Key, weight = (float)x.Count() }).ToList();
            float max = weightedCandidatesImmutable.Max(x => x.weight);
            List<(CellVariable, float)> weightedCandidatesMutable = new();
            foreach (var candidate in weightedCandidatesImmutable)
            {
                if (candidate.weight == max)
                {
                    weightedCandidatesMutable.Add(new(candidate.element, candidate.weight * bias));
                    continue;
                }
                weightedCandidatesMutable.Add(new(candidate.element, candidate.weight));
            }
            return GameManager.RandomElementWeighted(weightedCandidatesMutable.Select(x => x.Item1), weightedCandidatesMutable.Select(x => x.Item2).ToList());
        }

        public HashSet<CellVariable> GetPossibleCellVariables(Dictionary<Side, CellVariable> neighbours)
        {
            HashSet<CellVariable> options = new(cellVariables);
            foreach (var item in neighbours)
            {
                switch (item.Key)
                {
                    case Side.LEFT:
                        options.IntersectWith(item.Value.left);
                        break;
                    case Side.RIGHT:
                        options.IntersectWith(item.Value.right);
                        break;
                    case Side.UP:
                        options.IntersectWith(item.Value.up);
                        break;
                    case Side.DOWN:
                        options.IntersectWith(item.Value.down);
                        break;
                    default:
                        break;
                }
            }
            return options;
        }

        public List<CellVariable> GetAllCellVariables()
        {
            return new List<CellVariable>(cellVariables);
        }
    }
}
