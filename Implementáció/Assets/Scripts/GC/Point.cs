using UnityEngine;

namespace GC
{
    public enum PointType
    {
        Star, Planet, Black_Hole, Resource, None, Unknown, Any, Temporary
    }
    public class Point
    {
        public Vector2 position;
        public PointType type;

        public Point(float x, float y)
        {
            position = new Vector2(x, y);
            type = PointType.Unknown;
        }

        public Point(float x, float y, PointType type)
        {
            position = new Vector2(x, y);
            this.type = type;
        }

        public void SetType(PointType type)
        {
            this.type = type;
        }

        public void RandomizePosition(float amount)
        {
            position.x += Random.Range(-amount, amount);
            position.y += Random.Range(-amount, amount);
        }


        public float Distance(Point point)
        {
            return Vector2.Distance(position, point.position);
        }
        public override string ToString()
        {
            return position.ToString() + " type: " + type.ToString();
        }
    }
}
