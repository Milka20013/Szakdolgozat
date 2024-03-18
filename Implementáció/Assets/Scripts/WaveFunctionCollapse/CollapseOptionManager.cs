using ProjectCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC
{
    /// <summary>
    /// Class for some global options and functions used in WFC generation
    /// </summary>
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
        public void SetBias(string text)
        {
            if (float.TryParse(text, out float value))
            {
                if (value != 0)
                {
                    bias = value;
                }
            }
        }
        /// <summary>
        /// Calculate entropy based on the "easiness" to choose a cell according to their neighbor.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Return a random cell variable that can neighbor all cells in the neighbours parameter.
        /// If a wildcard is chosen it can always be returned if the neighboring situation is too strict.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
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
            return ProjectManager.RandomElementWeighted(options);
        }
        /// <summary>
        /// Get a random cell variable based on the connections between neighboring cells.
        /// The neighboring connections and weight are based on how much times a cell neighbours an other.
        /// In case of a 1 dimensional example: OXYXXOXYYYYYOXOXOXOXOO  Y neighbors itself much more than X so the likeliness
        /// Of a cell becoming Y when neighboring a Y is higher than becoming an X, even thogh X appears more overall.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
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
            return ProjectManager.RandomElementWeighted(weightedCandidatesMutable.Select(x => x.Item1), weightedCandidatesMutable.Select(x => x.Item2).ToList());
        }
        /// <summary>
        /// Filter the possible cells based on its neighbors. Only cells that can neighbor every neighbor can be in the list.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Used in earlier versions as a conversion from array to list.
        /// #REMOVE
        /// </summary>
        /// <returns></returns>
        public List<CellVariable> GetAllCellVariables()
        {
            return new List<CellVariable>(cellVariables);
        }
    }
}
