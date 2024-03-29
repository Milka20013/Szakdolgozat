
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
            float number = Random.Range(0, collection.Sum(x => x.GetWeight()));
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
            float number = Random.Range(0, weights.Sum());
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
        public static int Mod(int a, int b)
        {
            return System.Math.Abs((a * b) + a) % b;
        }
    }
}
