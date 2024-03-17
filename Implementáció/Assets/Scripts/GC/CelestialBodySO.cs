using UnityEngine;

namespace GC
{
    [CreateAssetMenu(menuName = "CelestialBody")]
    public class CelestialBodySO : ScriptableObject
    {
        public GameObject prefab;
        public PointType type;
    }
}
