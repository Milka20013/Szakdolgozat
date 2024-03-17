using UnityEngine;

namespace GC
{
    public class CelestialBody : MonoBehaviour
    {
        protected Point point;
        [SerializeField] protected CelestialBodySO data;
        protected Vector2 boundaries;
        protected float spacing;
        public virtual void Init(Point point, Vector2 boundaries, float spacing)
        {
            this.point = point;
            this.boundaries = boundaries;
            this.spacing = spacing;
            transform.position *= spacing;
        }
    }
}
