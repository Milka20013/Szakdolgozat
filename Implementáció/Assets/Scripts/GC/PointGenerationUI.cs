using UnityEngine;

namespace GC
{
    public class PointGenerationUI : MonoBehaviour
    {
        [SerializeField] private PointGeneration pointGeneration;

        public void SetNumberOfStars(string text)
        {
            if (int.TryParse(text, out int result))
            {
                pointGeneration.maxNumberOfStars = result;
            }
        }
        public void SetNumberOfBlackHoles(string text)
        {
            if (int.TryParse(text, out int result))
            {
                pointGeneration.maxNumberOfBlackHoles = result;
            }
        }
        public void SetBlackHoleClearZoneRadius(string text)
        {
            if (float.TryParse(text, out float result))
            {
                pointGeneration.blackHoleClearZoneRadius = result;
            }
        }

        public void SetResourceDensity(string text)
        {
            if (float.TryParse(text, out float result))
            {
                pointGeneration.resourceDensity = result;
            }
        }
        public void SetDimensionX(string text)
        {
            if (int.TryParse(text, out int result))
            {
                pointGeneration.dimension.x = result;
            }
        }
        public void SetDimensionY(string text)
        {
            if (int.TryParse(text, out int result))
            {
                pointGeneration.dimension.y = result;
            }
        }
        public void SetSpacing(string text)
        {
            if (float.TryParse(text, out float result))
            {
                pointGeneration.spacing = result;
            }
        }
        public void SetPositionNoise(string text)
        {
            if (float.TryParse(text, out float result))
            {
                pointGeneration.positionNoise = result;
            }
        }
    }
}
