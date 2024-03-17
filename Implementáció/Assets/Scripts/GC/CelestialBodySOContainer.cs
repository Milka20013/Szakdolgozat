using System.Linq;
using UnityEngine;
namespace GC
{
    [CreateAssetMenu(menuName = "Container/CelestialBody")]
    public class CelestialBodySOContainer : ScriptableObject
    {
        public CelestialBodySO[] celestialBodies;

        public GameObject GetPrefab(Point point)
        {
            var cb = celestialBodies.Where(x => x.type == point.type).FirstOrDefault();
            if (cb == null)
            {
                return null;
            }
            return cb.prefab;
        }
    }
}