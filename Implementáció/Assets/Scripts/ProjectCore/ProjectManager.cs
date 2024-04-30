using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ProjectCore
{
    /// <summary>
    /// Static helper class for commonly used functions
    /// </summary>
    public static class ProjectManager
    {
        public static System.Random rnd = new(500);
        public static void SetSeed(int seed)
        {
            rnd = new System.Random(seed);
        }
        public static T RandomElement<T>(IEnumerable<T> collection, out int index)
        {
            System.Random rnd = new();
            index = rnd.Next(collection.Count());
            return collection.ElementAt(index);
        }
        public static T RandomElement<T>(IEnumerable<T> collection)
        {
            System.Random rnd = new();
            int index = rnd.Next(collection.Count());
            return collection.ElementAt(index);
        }
        public static T RandomElementWeighted<T>(IEnumerable<T> collection) where T : IWeighted
        {
            float number = UnityEngine.Random.Range(0, collection.Sum(x => x.GetWeight()));
            foreach (var item in collection)
            {
                number -= item.GetWeight();
                if (number <= 0)
                {
                    return item;
                }
            }
            return default;
        }
        public static T RandomElementWeighted<T>(IEnumerable<T> collection, IEnumerable<float> weights)
        {
            float number = UnityEngine.Random.Range(0, weights.Sum());
            for (int i = 0; i < collection.Count(); i++)
            {
                number -= weights.ElementAt(i);
                if (number <= 0f)
                {
                    return collection.ElementAt(i);
                }
            }
            return default;
        }
        public static void ShuffleArray<T>(T[] array)
        {
            int n = array.Length;
            System.Random rng = new();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }
        public static Vector2Int[] GetNeighborPositionsOfPoint(Vector2Int position, int width, int height)
        {
            Vector2Int[] positions = new Vector2Int[4];
            positions[0] = new(Mod(position.x - 1, width), position.y);
            positions[1] = new(Mod(position.x + 1, width), position.y);
            positions[2] = new(position.x, Mod(position.y - 1, height));
            positions[3] = new(position.x, Mod(position.y + 1, height));
            return positions;
        }
        public static KeyValuePair<T1, T2> MinInDictionaryRandom<T1, T2>(Dictionary<T1, T2> dict) where T2 : IComparable
        {
            int firstValue = rnd.Next(0, dict.Count);
            T1[] keys = dict.Keys.ToArray();
            KeyValuePair<T1, T2> minPair = new(keys[firstValue], dict[keys[firstValue]]);

            for (int i = firstValue; i < firstValue + keys.Length; i++)
            {
                T1 key = keys[i % dict.Count];
                T2 value = dict[key];

                if (value.CompareTo(minPair.Value) < 0)
                {
                    minPair = new KeyValuePair<T1, T2>(key, value);
                }
            }

            return minPair;
        }
        public static int Mod(int a, int b)
        {
            return System.Math.Abs((a * b) + a) % b;
        }
    }
}
