using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class CellVariable : IWeighted
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
