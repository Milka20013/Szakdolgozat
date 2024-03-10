using ProjectCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC
{
    public class CellVariable : IWeighted
    {
        public CellVariable()
        {

        }
        public string name;
        public Sprite sprite;
        public float weight;
        public List<CellVariable> left = new();
        public List<CellVariable> right = new();
        public List<CellVariable> up = new();
        public List<CellVariable> down = new();

        public float GetWeight()
        {
            return weight;
        }

        public CellVariable PreferredOption(List<CellVariable> options)
        {
            float[] weights = new float[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                weights[i] = LocalWeightConnection.GetWeightOfConnection(this, options[i]);
            }
            float max = weights.Max();
            if (max == -1)
            {
                return null;
            }
            List<CellVariable> candidates = new();
            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] == max)
                {
                    candidates.Add(options[i]);
                }
            }
            return ProjectManager.RandomElement(candidates);
        }
    }
}
