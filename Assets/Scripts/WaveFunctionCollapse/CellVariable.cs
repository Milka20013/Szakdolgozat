using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "CellVariable")]
    public class CellVariable : ScriptableObject, IWeighted
    {
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
    }
}
