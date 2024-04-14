using ProjectCore;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    /// <summary>
    /// The generator of the WFC algorithm
    /// </summary>
    public class ImageGrid : MonoBehaviour
    {
        public Vector2Int dimension;
        public Image imagePrefab;
        public string outputPath = "output";
        public Canvas canvas;
        public int maxIterations = 5;
        public PickModel pickModel;
        public bool animate;
        public bool cellMode;
        public bool useLocalWeights;
        private int currentIterations = 0;

        public UnityEvent onAlgorithmEnd;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI imageSizeText;

        private int scale;


        private ImageCell[,] cells;

        //public TextMeshProUGUI text;
        private void Awake()
        {
            scale = (int)imagePrefab.GetComponent<RectTransform>().rect.width;
            //Init();
        }

        //These methods are used by the UI in the generation scene to set the variables via user input.
        public void SetCellMode(bool value)
        {
            cellMode = value;
        }

        public void SetAnimateMode(bool value)
        {
            animate = value;
        }

        public void SetLocalWeightMode(bool value)
        {
            useLocalWeights = value;
        }

        public void SetPickModel(int model)
        {
            pickModel = (PickModel)model;
        }
        public void SetWidth(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result == 0)
                {
                    return;
                }
                dimension.x = result;
                RefreshImageSize();
            }
        }
        public void SetHeight(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result == 0)
                {
                    return;
                }
                dimension.y = result;
                RefreshImageSize();
            }
        }

        private void RefreshImageSize()
        {
            imageSizeText.text = (scale * dimension.x).ToString() + "x" + (scale * dimension.y).ToString();
        }
        /// <summary>
        /// Init the algorithm based on the cellMode variable
        /// </summary>
        public void Init()
        {
            cells = new ImageCell[dimension.x, dimension.y];
            if (cellMode)
            {
                InitCellMode();
            }
            else
            {
                InitNonCellMode();
            }
            RunAlgorithm();
        }
        /// <summary>
        /// Init the algorithm with cells. Every cell gets an individual image component and it is initialized into the scene.
        /// Much slower than the non cell mode, but it can be animated.
        /// </summary>
        private void InitCellMode()
        {
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    var image = Instantiate(imagePrefab);
                    image.transform.SetParent(canvas.transform);
                    image.transform.localScale = new(1, 1);
                    image.transform.localPosition = new Vector3((x - (dimension.x) / 2 + 0.5f) * scale, (y - dimension.y / 2 + 0.5f) * scale);
                    cells[x, y] = new ImageCell(image);
                }
            }
        }
        /// <summary>
        /// Init the algorithm with virtual cells. Every cell gets the same image and they can write their data onto it.
        /// Much faster than the cell method, but it can't be animated.
        /// </summary>

        private void InitNonCellMode()
        {
            int blockWidth = CollapseOptionManager.Instance.cellVariables[0].sprite.texture.width;
            int blockHeight = CollapseOptionManager.Instance.cellVariables[0].sprite.texture.height;
            if (dimension.x % blockWidth != 0 || dimension.y % blockHeight != 0)
            {
                Debug.LogError("Block dimensions are not compatible");
                return;
            }
            dimension = new(dimension.x / blockWidth, dimension.y / blockHeight);
            var image = Instantiate(imagePrefab);
            Texture2D texture = new(dimension.x * blockWidth, dimension.y * blockHeight);
            texture.filterMode = FilterMode.Point;
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new(0.5f, 0.5f));
            image.sprite = sprite;
            image.transform.SetParent(canvas.transform);
            image.transform.localPosition = new(0, 0);
            image.transform.localScale = new(dimension.x, dimension.y);

            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    cells[x, y] = new VirtualImageCell(image, new(x, y));
                }
            }
            onAlgorithmEnd.RemoveListener(ApplyImage);
            onAlgorithmEnd.AddListener(ApplyImage);
        }
        /// <summary>
        /// This is a method used on the end of the algorithm if the mode is not cell mode.
        /// It is necessary as the resulting image has to be rendered at the end.
        /// Multiple calls of apply would cause performance issues.
        /// </summary>
        private void ApplyImage()
        {
            cells[0, 0].image.sprite.texture.Apply();
        }
        /// <summary>
        /// Restart the algorithm and reset the iteration counter.
        /// TO-DO: fix this method
        /// </summary>
        public void HardRestart()
        {
            //currentIterations = 0;
            Clear();
            RunAlgorithm();
        }
        /// <summary>
        /// Restart the algorithm
        /// </summary>
        public void Restart()
        {
            StopAllCoroutines();
            Clear();
            RunAlgorithm();
        }
        /// <summary>
        /// Clear the grid by reseting all the cells from startX and startY
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
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
        /// <summary>
        /// Run the algorithm based on the PickModel state
        /// As the algorithm can repeat itself, number of executions are limited to maxIterations.
        /// </summary>
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
        /// <summary>
        /// Go through the cells vertically starting at the bottom left corner
        /// </summary>
        /// <returns></returns>
        IEnumerator VerticalModel()
        {
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    if (CollapseCell(x, y) == null)
                    {
                        onAlgorithmEnd.Invoke();
                        Clear(x, y);
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
            onAlgorithmEnd.Invoke();

        }
        /// <summary>
        /// Go through the cells horizontally starting at the bottom left corner
        /// </summary>
        /// <returns></returns>
        IEnumerator HorizontalModel()
        {
            for (int y = 0; y < dimension.y; y++)
            {
                for (int x = 0; x < dimension.x; x++)
                {
                    if (CollapseCell(x, y) == null)
                    {
                        onAlgorithmEnd.Invoke();
                        Clear(x, y);
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
            onAlgorithmEnd.Invoke();

        }
        /// <summary>
        /// Go through the cells according to their entropy. The lowest entropy cells get picked, randomly if multiple exists.
        /// TO-DO: optimize the complexity (it is O(n^4) currently)
        /// </summary>
        /// <returns></returns>
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
                        onAlgorithmEnd.Invoke();
                        Clear();
                        yield break;
                    }
                    var equalEntropyCells = cells.Cast<ImageCell>().Select(x => x).Where(x => x.Entropy() == min);
                    var cell = ProjectManager.RandomElement(equalEntropyCells);
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
                        onAlgorithmEnd.Invoke();
                        Clear();
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
            onAlgorithmEnd.Invoke();

        }

        /// <summary>
        /// Go through the cells in a random order, and collapse them
        /// </summary>
        /// <returns></returns>
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
                ProjectManager.ShuffleArray(indexArray[x]);
            }
            int[] randomXIndices = new int[dimension.x];
            for (int i = 0; i < dimension.x; i++)
            {
                randomXIndices[i] = i;
            }
            ProjectManager.ShuffleArray(randomXIndices);
            for (int x = 0; x < dimension.y; x++)
            {
                for (int y = 0; y < dimension.x; y++)
                {
                    if (CollapseCell(randomXIndices[y], indexArray[randomXIndices[y]][x]) == null)
                    {
                        onAlgorithmEnd.Invoke();
                        Clear();
                        yield break;
                    }
                    if (animate)
                    {
                        yield return null;
                    }
                }
            }
            onAlgorithmEnd.Invoke();

        }
        /// <summary>
        /// Determine the final state of the cell, and set its neighbors.
        /// </summary>
        /// <param name="x">x position of the cell</param>
        /// <param name="y">y position of the cell</param>
        /// <returns></returns>
        private CellVariable CollapseCell(int x, int y)
        {
            CellVariable cellVariable;
            if (!useLocalWeights)
            {
                cellVariable = cells[x, y].CollapseCell();
            }
            else
            {
                cellVariable = cells[x, y].CollapseCellWithLocalWeights();
            }

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

        /// <summary>
        /// Exports the result into an image named imageOutput.png.
        /// TO-DO: update this function to export the image when it is not composed by cells.
        /// </summary>
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
            //text.text = "ready";
        }

        public void ExportAsTxt()
        {
            StringBuilder builder = new();
            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    builder.Append(i.ToString() + "," + j.ToString() + "," + cells[i, j].name);
                    if (j != dimension.y - 1)
                    {
                        builder.Append(" ");
                    }
                }
                builder.Append("\n");
            }
            File.WriteAllText(Path.Combine(Path.Combine(Application.streamingAssetsPath, outputPath), "output.txt"), builder.ToString());
        }

        private void Update()
        {
            //inputs to test stuff in editor
            //#REMOVE
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExportAsTxt();
                ExportImage();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                cells[0, 0].image.sprite.texture.Apply();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }
}
