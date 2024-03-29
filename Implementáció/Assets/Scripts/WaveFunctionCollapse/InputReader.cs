using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    /// <summary>
    /// Input reader used in WFC algorithm.
    /// </summary>
    public class InputReader : MonoBehaviour
    {

        public Texture2D referenceTexture;
        private List<Texture2D> textures = new();
        public string inputPath = "input";
        public Image image;
        public ImageContext imageContext;
        public Transform container;
        public TextMeshProUGUI readingText;

        private void Awake()
        {
            ReadImagesFromFolder();
        }
        /// <summary>
        /// Read all the png images placed in the streamingAssetsPath directory
        /// </summary>
        private void ReadImagesFromFolder()
        {
            string[] fileNames = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, inputPath), "*.png").ToArray();
            foreach (var fileName in fileNames)
            {
                ConvertImageToTextureFromFile(fileName, fileNames.Length);
            }
            InstantiateImages();
        }
        /// <summary>
        /// Spawn the images into the scene
        /// </summary>
        private void InstantiateImages()
        {
            for (int i = 0; i < textures.Count; i++)
            {
                var img = Instantiate(image, container);
                img.GetComponent<ImageButton>().imageContext = imageContext;
                img.sprite = Sprite.Create(textures[i], new Rect(0f, 0f, textures[i].width, textures[i].height), new(0.5f, 0.5f));

                //ConvertReferenceTextureToBlock();

                //SetPositionedCellVariables();

                //CollapseOptionManager.Instance.cellVariables = positionedCellVariables.Select(x => x.cellVariable).ToArray();
            }
        }
        /// <summary>
        /// Convert png image to texture for the generation
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="countOfFiles"></param>
        private void ConvertImageToTextureFromFile(string fileName, int countOfFiles)
        {
            var bytes = File.ReadAllBytes(fileName);
            Texture2D texture = new(1, 1);
            if (!ImageConversion.LoadImage(texture, bytes))
            {
                Debug.LogError("Couldn't load " + fileName + " to texture");
            }
            texture.filterMode = FilterMode.Point;
            textures.Add(texture);
            readingText.gameObject.SetActive(false);
        }
    }
}
