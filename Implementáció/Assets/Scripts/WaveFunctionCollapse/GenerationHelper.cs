using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WFC
{
    public class GenerationHelper : MonoBehaviour
    {
        [SerializeField] private string outputPath;
        public void CountBlocks()
        {
            string text = File.ReadAllText(Path.Combine(Path.Combine(Application.streamingAssetsPath, outputPath), "output.txt"));
            var blocks = text.Split();
            Dictionary<string, int> blockAndValuePairs = new();
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].Length == 0)
                {
                    continue;
                }
                string blockType = blocks[i].Split(',')[2];
                if (!blockAndValuePairs.TryAdd(blockType, 1))
                {
                    blockAndValuePairs[blockType] = blockAndValuePairs[blockType] + 1;
                }
            }
            foreach (var item in blockAndValuePairs)
            {
                Debug.Log($"{item.Key} : {item.Value}");
            }
        }
    }
}
