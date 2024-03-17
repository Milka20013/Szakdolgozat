using UnityEngine;

namespace ProjectCore
{
    public static class Noise
    {
        public static float[,] GeneratePerlinNoiseMap(int width, int height, float scale)
        {
            float[,] map = new float[width, height];
            if (scale == 0f)
            {
                scale = 0.0001f;
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float sampleX = i / scale;
                    float sampleY = j / scale;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleY);
                    map[i, j] = perlin;
                }
            }
            return map;
        }
    }
}
