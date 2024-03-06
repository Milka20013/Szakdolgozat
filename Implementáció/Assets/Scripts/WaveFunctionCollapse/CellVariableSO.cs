using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "CellVariable")]
    public class CellVariableSO : ScriptableObject, IWeighted
    {
        public Sprite sprite;
        public float weight;
        public List<CellVariableSO> left = new();
        public List<CellVariableSO> right = new();
        public List<CellVariableSO> up = new();
        public List<CellVariableSO> down = new();

        public float GetWeight()
        {
            return weight;
        }
    }
}
