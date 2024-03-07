using CoreGame;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    public enum Side
    {
        LEFT, RIGHT, UP, DOWN
    }
    public enum PickModel
    {
        VERTICAL, HORIZONTAL, SHANNON_ENTROPY, RANDOM
    }
    public class ImageGrid : MonoBehaviour
    {
        public Vector2Int dimension;
        public Image imagePrefab;
        public string outputPath = "output";
        public Canvas canvas;
        public int maxIterations = 5;
        public PickModel pickModel;
        public bool animate;
        private int currentIterations = 0;

        private float scale;

        private ImageCell[,] cells;

        public TextMeshProUGUI text;
        private void Awake()
        {
            scale = imagePrefab.GetComponent<RectTransform>().rect.width;
            cells = new ImageCell[dimension.x, dimension.y];
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    var image = Instantiate(imagePrefab);
                    image.transform.SetParent(canvas.transform);
                    image.transform.position = new Vector3(x * scale, y * scale) + transform.position;
                    cells[x, y] = new(image.GetComponent<Image>());
                }
            }
        }
        private void Start()
        {
            RunAlgorithm();
        }
        public void HardRestart()
        {
            //currentIterations = 0;
            Clear();
            RunAlgorithm();
        }
        public void Restart()
        {
            Clear();
            RunAlgorithm();
        }

        public void Clear(int startX = 0, int startY = 0)
        {
            for (int x = startX; x < dimension.x; x++)
            {
                for (int y = startY; y < dimension.y; y++)
                {
                    cells[x, y].Reset(imagePrefab.sprite);
                }
            }
        }
        public void RunAlgorithm()
        {
            currentIterations++;
            if (currentIterations > maxIterations)
            {
                return;
            }
            switch (pickModel)
            {
                case PickModel.VERTICAL:
                    StartCoroutine(nameof(VerticalModel));
                    break;
                case PickModel.HORIZONTAL:
                    StartCoroutine(nameof(HorizontalModel));
                    break;
                case PickModel.SHANNON_ENTROPY:
                    StartCoroutine(nameof(ShannonModel));
                    break;
                case PickModel.RANDOM:
                    StartCoroutine(nameof(RandomModel));
                    break;
                default:
                    break;
            }

        }
        IEnumerator VerticalModel()
        {
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    if (CollapseCell(x, y) == null)
                    {
                        Clear(x, y);
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
        }
        IEnumerator HorizontalModel()
        {
            for (int y = 0; y < dimension.y; y++)
            {
                for (int x = 0; x < dimension.x; x++)
                {
                    if (CollapseCell(x, y) == null)
                    {
                        Clear(x, y);
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
        }

        IEnumerator ShannonModel()
        {
            float[,] entropies = new float[dimension.x, dimension.y];
            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    for (int x = 0; x < dimension.x; x++)
                    {
                        for (int y = 0; y < dimension.y; y++)
                        {
                            entropies[x, y] = cells[x, y].Entropy();
                        }
                    }
                    float min = entropies.Cast<float>().Min();
                    if (float.IsPositiveInfinity(min))
                    {
                        yield break;
                    }
                    var equalEntropyCells = cells.Cast<ImageCell>().Select(x => x).Where(x => x.Entropy() == min);
                    var cell = GameManager.RandomElement(equalEntropyCells);
                    int xIndex = 0;
                    int yIndex = 0;
                    for (int x = 0; x < dimension.x; x++)
                    {
                        for (int y = 0; y < dimension.y; y++)
                        {
                            if (cells[x, y] == cell)
                            {
                                xIndex = x;
                                yIndex = y;
                                break;
                            }
                        }
                    }
                    if (CollapseCell(xIndex, yIndex) == null)
                    {
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
        }
        IEnumerator RandomModel()
        {
            int[][] indexArray = new int[dimension.x][];
            for (int x = 0; x < dimension.x; x++)
            {
                indexArray[x] = new int[dimension.y];
                for (int y = 0; y < dimension.y; y++)
                {
                    indexArray[x][y] = y;
                }
                GameManager.ShuffleArray(indexArray[x]);
            }
            int[] randomXIndices = new int[dimension.x];
            for (int i = 0; i < dimension.x; i++)
            {
                randomXIndices[i] = i;
            }
            GameManager.ShuffleArray(randomXIndices);
            for (int x = 0; x < dimension.y; x++)
            {
                for (int y = 0; y < dimension.x; y++)
                {
                    if (CollapseCell(randomXIndices[y], indexArray[randomXIndices[y]][x]) == null)
                    {
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
        }
        private CellVariable CollapseCell(int x, int y)
        {
            CellVariable cellVariable = cells[x, y].CollapseCell();

            if (cellVariable == null)
            {
                Debug.Log($"Iteration failed at {x},{y}");
                Restart();
                return null;
            }

            if (x > 0)
            {
                cells[x - 1, y].SetNeighbour(Side.LEFT, cellVariable);
            }
            if (x < dimension.x - 1)
            {
                cells[x + 1, y].SetNeighbour(Side.RIGHT, cellVariable);
            }
            if (y > 0)
            {
                cells[x, y - 1].SetNeighbour(Side.DOWN, cellVariable);
            }
            if (y < dimension.y - 1)
            {
                cells[x, y + 1].SetNeighbour(Side.UP, cellVariable);
            }
            return cellVariable;
        }

        public void ExportImage()
        {
            int xDimension = dimension.x * cells[0, 0].image.sprite.texture.width;
            int yDimension = dimension.y * cells[0, 0].image.sprite.texture.height;
            int width = cells[0, 0].image.sprite.texture.width;
            int height = cells[0, 0].image.sprite.texture.height;

            Texture2D targetTexture = new(xDimension, yDimension);
            targetTexture.filterMode = FilterMode.Point;
            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    var pixels = cells[i, j].image.sprite.texture.GetPixels();
                    targetTexture.SetPixels(width * i, height * j, width, height, pixels);
                }
            }
            targetTexture.Apply();
            var bytes = ImageConversion.EncodeToPNG(targetTexture);
            File.WriteAllBytes(Path.Combine(Path.Combine(Application.streamingAssetsPath, outputPath), "imageOutput.png"), bytes);
            text.text = "ready";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExportImage();
            }
        }
    }
}
