using ProjectCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    /// <summary>
    /// Converter for the reference image in WFC algorithm.
    /// </summary>
    public class TextureConverter : MonoBehaviour
    {
        private Texture2D referenceTexture;
        [SerializeField] Image image;

        public Vector2Int blockDimensions = new(1, 1);
        public bool cellCanNeighborItself;
        public bool continuousBlocks;

        [Header("UI")]
        [SerializeField] private bool useUI;
        [SerializeField] private TextMeshProUGUI imageSizeText;
        [SerializeField] private TextMeshProUGUI blockCountText;
        [SerializeField] private TextMeshProUGUI successText;
        [SerializeField] private Button checkBlocksButton;
        [SerializeField] private GameObject nextButton;
        [SerializeField] private GameObject algorithmParameters;
        [SerializeField] private GameObject blockGenerationParameters;
        [SerializeField] private BlockContext blockContext;


        private int numberOfBlocksPerLine;
        private int numberOfBlocksPerColoumn;
        private List<PositionedBlock> positionedBlocks = new();
        private List<PositionedCellVariable> positionedCellVariables = new();

        private void Awake()
        {
            if (DataContainer.ChosenImageSprite != null)
            {
                referenceTexture = DataContainer.ChosenImageSprite.texture;
                image.sprite = DataContainer.ChosenImageSprite;
            }
            else
            {
                referenceTexture = image.sprite.texture;
            }
            var width = 1;
            var height = 1;

            if (useUI)
            {
                imageSizeText.text = width.ToString() + "x" + height.ToString();
            }
            blockDimensions = new(width, height);
        }
        //These methods are used in UI
        public void SetCellNeighbor(bool value)
        {
            cellCanNeighborItself = value;
        }
        public void SetContinuousBlock(bool value)
        {
            continuousBlocks = value;
            RefreshBlockCount();
        }
        public void SetWidth(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result == 0 || result > referenceTexture.width)
                {
                    return;
                }
                blockDimensions.x = result;
                RefreshBlockCount();
            }
        }
        public void SetHeight(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result == 0 || result > referenceTexture.height)
                {
                    return;
                }
                blockDimensions.y = result;
                RefreshBlockCount();
            }
        }

        /// <summary>
        /// Refresh the block count on the UI
        /// It is not accurate, but a high bound to the number of blocks generated.
        /// </summary>
        private void RefreshBlockCount()
        {
            bool wrong = false;

            if (continuousBlocks)
            {
                if (referenceTexture.width % blockDimensions.x != 0)
                {
                    wrong = true;
                }
                if (referenceTexture.height % blockDimensions.y != 0)
                {
                    wrong = true;

                }
                numberOfBlocksPerLine = referenceTexture.height / blockDimensions.y;
                numberOfBlocksPerColoumn = referenceTexture.width / blockDimensions.x;
            }
            else
            {
                numberOfBlocksPerLine = referenceTexture.height - blockDimensions.y + 1;
                numberOfBlocksPerColoumn = referenceTexture.width - blockDimensions.x + 1;
            }

            blockCountText.text = (numberOfBlocksPerLine * numberOfBlocksPerColoumn).ToString();
            if (wrong)
            {
                blockCountText.text = "NON INTEGER";
            }
        }
        /// <summary>
        /// Start the converting procedure
        /// </summary>
        public void Init()
        {
            try
            {
                ConvertReferenceTextureToBlock();
                SetPositionedCellVariables();
                if (useUI)
                {
                    successText.text = positionedBlocks.Count.ToString() + " block(s) generated";
                    checkBlocksButton.gameObject.SetActive(true);
                    if (positionedBlocks.Count >= 40)
                    {
                        checkBlocksButton.interactable = false;
                    }
                    nextButton.SetActive(true);
                }

                CollapseOptionManager.Instance.cellVariables = positionedCellVariables.Select(x => x.cellVariable).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                successText.text = e.Message;
            }
        }
        /// <summary>
        /// Used in UI
        /// </summary>
        public void NextParameters()
        {
            blockGenerationParameters.SetActive(false);
            algorithmParameters.SetActive(true);
        }
        private void ConvertReferenceTextureToBlock()
        {
            positionedBlocks = new();
            positionedCellVariables = new();
            if (continuousBlocks)
            {
                BlockAsGridImageConversion();
                numberOfBlocksPerLine = referenceTexture.height / blockDimensions.y;
                numberOfBlocksPerColoumn = referenceTexture.width / blockDimensions.x;
            }
            else
            {
                BlockAsAnyPositionImageConversion();
                numberOfBlocksPerLine = referenceTexture.height - blockDimensions.y + 1;
                numberOfBlocksPerColoumn = referenceTexture.width - blockDimensions.x + 1;
            }
        }
        /// <summary>
        /// Create the cellVariables and their neighbor connections
        /// </summary>
        private void SetPositionedCellVariables()
        {
            positionedCellVariables = new();
            foreach (var pBlock in positionedBlocks)
            {
                var cellV = new CellVariable
                {
                    sprite = Sprite.Create(pBlock.block, new Rect(0, 0, pBlock.block.width, pBlock.block.height), new(0.5f, 0.5f)),
                    weight = pBlock.occurence,
                };
                positionedCellVariables.Add(new(cellV, pBlock.positions));
            }
            //lazy to name in the previous
            for (int i = 0; i < positionedCellVariables.Count; i++)
            {
                positionedCellVariables[i].cellVariable.name = i.ToString();
            }
            for (int i = 0; i < positionedBlocks.Count; i++)
            {
                SetNeighbours(positionedCellVariables[i], GetNeighboursPosition(positionedBlocks[i]));
                if (cellCanNeighborItself)
                {
                    SetCellAsAnyNeighbour(positionedCellVariables[i], positionedCellVariables[i]);
                }
            }
        }
        /// <summary>
        /// Convert the image like it is a grid. A 4x4 grid has 16 1x1 cells and 4 2x2 cells.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BlockAsGridImageConversion()
        {
            if (referenceTexture.width % blockDimensions.x != 0)
            {
                throw new Exception("Width is not divisible by dimension x");
            }
            if (referenceTexture.height % blockDimensions.y != 0)
            {
                throw new Exception("Height is not divisible by dimension y");
            }
            for (int i = 0; i < referenceTexture.width / blockDimensions.x; i++)
            {
                for (int j = 0; j < referenceTexture.height / blockDimensions.y; j++)
                {
                    var texture = ReadBlockFromReferenceTexture(new(i * blockDimensions.x, j * blockDimensions.y));
                    //if the block exists, then put the position into the block.
                    //if not, add the block to the list
                    if (IsTransparentBlock(texture))
                    {
                        continue;
                    }
                    if (!CheckIfBlockExists(texture, out var block))
                    {
                        positionedBlocks.Add(new(texture, new(i, j)));
                    }
                    else
                    {
                        block.occurence++;
                        block.positions.Add(new(i, j));
                    }
                }
            }
        }
        /// <summary>
        /// Convert the image like it is not a grid. A 4x4 grid has 16 1x1 cells and 9 2x2 cells.
        /// Any NxN region can be chosed in the grid to create cells.
        /// TO-DO: make this version viable, currently it should not be used
        /// #REMOVE ?
        /// </summary>
        private void BlockAsAnyPositionImageConversion()
        {
            for (int i = 0; i < referenceTexture.width - blockDimensions.x + 1; i++)
            {
                for (int j = 0; j < referenceTexture.height - blockDimensions.y + 1; j++)
                {
                    var texture = ReadBlockFromReferenceTexture(new(i, j));
                    //if the block exists, then put the position into the block.
                    //if not, add the block to the list
                    if (IsTransparentBlock(texture))
                    {
                        continue;
                    }
                    if (!CheckIfBlockExists(texture, out var block))
                    {
                        positionedBlocks.Add(new(texture, new(i, j)));
                    }
                    else
                    {
                        block.occurence++;
                        block.positions.Add(new(i, j));
                    }
                }
            }
        }
        /// <summary>
        /// Hash the image, so it can be checked whether an image block was already checked.
        /// Probably faster than pixel-by-pixel comparison of all of the previous images. :)
        /// </summary>
        /// <param name="block"></param>
        /// <param name="positionedBlock"></param>
        /// <returns></returns>
        private bool CheckIfBlockExists(Texture2D block, out PositionedBlock positionedBlock)
        {
            SHA256 sha = SHA256.Create();
            string shastr = BitConverter.ToString(sha.ComputeHash(block.GetRawTextureData()));
            for (int i = 0; i < positionedBlocks.Count; i++)
            {
                if (positionedBlocks[i].Hash == shastr)
                {
                    positionedBlock = positionedBlocks[i];
                    return true;
                }
            }
            positionedBlock = null;
            return false;
        }
        /// <summary>
        /// Read a NxM dimension block defined in dimension variable from the reference, starting from the up left corner.
        /// </summary>
        /// <param name="upLeftCorner"></param>
        /// <returns></returns>
        private Texture2D ReadBlockFromReferenceTexture(Vector2Int upLeftCorner)
        {
            Texture2D block = new(blockDimensions.x, blockDimensions.y, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            for (int x = upLeftCorner.x; x < upLeftCorner.x + blockDimensions.x; x++)
            {
                for (int y = upLeftCorner.y; y < upLeftCorner.y + blockDimensions.y; y++)
                {
                    block.SetPixel(x, y, referenceTexture.GetPixel(x, y));
                }
            }
            block.Apply();
            return block;
        }
        /// <summary>
        /// Get the position of the neighbors. It is calculated as the block were on a torus
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private Vector2Int[] GetNeighboursPosition(PositionedBlock block)
        {
            Vector2Int[] positions = new Vector2Int[block.positions.Count * 4];
            for (int i = 0; i < block.positions.Count; i++)
            {
                positions[4 * i + 0] = (new(ProjectManager.Mod(block.positions[i].x - 1, numberOfBlocksPerColoumn), block.positions[i].y));
                positions[4 * i + 1] = (new(ProjectManager.Mod(block.positions[i].x + 1, numberOfBlocksPerColoumn), block.positions[i].y));
                positions[4 * i + 2] = (new(block.positions[i].x, ProjectManager.Mod(block.positions[i].y - 1, numberOfBlocksPerLine)));
                positions[4 * i + 3] = (new(block.positions[i].x, ProjectManager.Mod(block.positions[i].y + 1, numberOfBlocksPerLine)));
            }

            return positions;
        }
        /// <summary>
        /// Search the cellvariables on the positions, and assign those the cellVariable given as a paramater.
        /// The neighbors get the cellVariable as their negihbor too.
        /// </summary>
        /// <param name="positionedCellVariable"></param>
        /// <param name="neighboursPositions"></param>
        private void SetNeighbours(PositionedCellVariable positionedCellVariable, Vector2Int[] neighboursPositions)
        {
            HashSet<CellVariable>[] directionalNeighbours = new HashSet<CellVariable>[4];
            for (int i = 0; i < directionalNeighbours.Length; i++)
            {
                directionalNeighbours[i] = new();
            }
            for (int i = 0; i < positionedCellVariable.positions.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var neighbor = positionedCellVariables
                                                        .Where(x => HasPosition(x, neighboursPositions[j + i * 4]))
                                                        .Select(x => x.cellVariable).FirstOrDefault();
                    directionalNeighbours[j].Add(neighbor);
                    LocalWeightConnection.CreateOrRegister(neighbor, positionedCellVariable.cellVariable);
                }
            }
            positionedCellVariable.cellVariable.left = directionalNeighbours[0].ToList();
            positionedCellVariable.cellVariable.right = directionalNeighbours[1].ToList();
            positionedCellVariable.cellVariable.up = directionalNeighbours[2].ToList();
            positionedCellVariable.cellVariable.down = directionalNeighbours[3].ToList();
        }
        /// <summary>
        /// Make it possible for a cell to neighbor itself any way
        /// </summary>
        /// <param name="targetCellVariable"></param>
        /// <param name="cell"></param>
        private void SetCellAsAnyNeighbour(PositionedCellVariable targetCellVariable, PositionedCellVariable cell)
        {
            if (!targetCellVariable.cellVariable.up.Contains(cell.cellVariable))
            {
                targetCellVariable.cellVariable.up.Add(cell.cellVariable);
            }
            if (!targetCellVariable.cellVariable.down.Contains(cell.cellVariable))
            {
                targetCellVariable.cellVariable.down.Add(cell.cellVariable);
            }
            if (!targetCellVariable.cellVariable.right.Contains(cell.cellVariable))
            {
                targetCellVariable.cellVariable.right.Add(cell.cellVariable);
            }
            if (!targetCellVariable.cellVariable.left.Contains(cell.cellVariable))
            {
                targetCellVariable.cellVariable.left.Add(cell.cellVariable);
            }

        }

        private bool IsTransparentBlock(Texture2D block)
        {
            var pixels = block.GetPixels();
            return pixels.Where(x => x.a == 0f).Count() == pixels.Length;
        }
        private bool PositionEquals(Vector2Int one, Vector2Int two)
        {
            if (one.x == two.x && one.y == two.y)
            {
                return true;
            }
            return false;
        }
        private bool HasPosition(PositionedCellVariable cellVariable, Vector2Int position)
        {
            for (int i = 0; i < cellVariable.positions.Count; i++)
            {
                if (PositionEquals(cellVariable.positions[i], position))
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowBlockContext()
        {
            blockContext.Show(positionedCellVariables);
        }
    }
}
