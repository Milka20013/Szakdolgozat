using CoreGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    public class InputReader : MonoBehaviour
    {
        private class PositionedBlock
        {
            public Texture2D block;
            public List<Vector2Int> positions = new();
            public int occurence;
            private string _hash = "";
            public string Hash
            {
                get
                {
                    if (_hash.Length == 0)
                    {
                        _hash = BitConverter.ToString(SHA256.Create().ComputeHash(block.GetRawTextureData()));
                    }
                    return _hash;
                }
                private set { }
            }
            public PositionedBlock(Texture2D block, Vector2Int position)
            {
                this.block = block;
                positions.Add(position);
                occurence = 1;
            }


        }
        private class PositionedCellVariable
        {
            public PositionedCellVariable(CellVariable cellVariable, List<Vector2Int> positions)
            {
                this.cellVariable = cellVariable;
                this.positions = positions;
            }
            public CellVariable cellVariable;
            public List<Vector2Int> positions;
        }
        public Texture2D referenceTexture;
        public Vector2Int blockDimensions;
        public string cellVariablesPath = "Assets/Resources/CellVariables/";
        public string spritePath = "Assets/Resources/";
        public bool cellCanNeighbourItself;
        public bool blockIsGrid;
        public Image image;
        private int numberOfBlocksPerLine;
        private int numberOfBlocksPerColoumn;
        private List<PositionedBlock> positionedBlocks = new();
        private List<PositionedCellVariable> positionedCellVariables = new();

#if UNITY_EDITOR
        /*public void CreateCells()
        {
            positionedBlocks = new();
            positionedCellVariables = new();
            if (blockIsGrid)
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
            for (int i = 0; i < positionedBlocks.Count; i++)
            {
                var img = ImageConversion.EncodeToPNG(positionedBlocks[i].block);
                File.WriteAllBytes(spritePath + "kep" + i + ".png", img);
                AssetDatabase.ImportAsset(spritePath + "kep" + i + ".png");
                var cellV = ScriptableObject.CreateInstance<CellVariableSO>();
                cellV.weight = positionedBlocks[i].occurence;
                positionedCellVariables.Add(new(cellV, positionedBlocks[i].positions));
                AssetDatabase.CreateAsset(cellV, cellVariablesPath + i + ".asset");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            for (int i = 0; i < positionedBlocks.Count; i++)
            {
                SetNeighbours(positionedCellVariables[i], GetNeighboursPosition(positionedBlocks[i]));
                if (cellCanNeighbourItself)
                {
                    SetCellAsAnyNeighbour(positionedCellVariables[i], positionedCellVariables[i]);
                }
            }
        }*/
#endif

        private void Awake()
        {
            ReadImagesFromFolder();
        }

        private void ReadImagesFromFolder()
        {
            string[] fileNames = Directory.GetFiles(Application.streamingAssetsPath, "*.png").ToArray();
            List<Texture2D> textures = new();
            foreach (var fileName in fileNames)
            {
                if (ConvertImageToTextureFromFile(fileName, out Texture2D texture))
                {
                    texture.filterMode = FilterMode.Point;
                    textures.Add(texture);
                    continue;
                }
                Debug.LogError("Couldn't load " + fileName + " to texture");
            }
            image.sprite = Sprite.Create(textures[0], new Rect(0f, 0f, textures[0].width, textures[0].height), new(0.5f, 0.5f));
            referenceTexture = textures[0];

            positionedBlocks = new();
            positionedCellVariables = new();
            if (blockIsGrid)
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
            foreach (var pBlock in positionedBlocks)
            {
                var cellV = new CellVariable
                {
                    sprite = Sprite.Create(pBlock.block, new Rect(0, 0, pBlock.block.width, pBlock.block.height), new(0.5f, 0.5f)),
                    weight = pBlock.occurence
                };
                positionedCellVariables.Add(new(cellV, pBlock.positions));
            }
            for (int i = 0; i < positionedBlocks.Count; i++)
            {
                SetNeighbours(positionedCellVariables[i], GetNeighboursPosition(positionedBlocks[i]));
                if (cellCanNeighbourItself)
                {
                    SetCellAsAnyNeighbour(positionedCellVariables[i], positionedCellVariables[i]);
                }
            }

            CollapseOptionManager.Instance.cellVariables = positionedCellVariables.Select(x => x.cellVariable).ToArray();
        }

        private bool ConvertImageToTextureFromFile(string filename, out Texture2D texture)
        {
            var bytes = File.ReadAllBytes(filename);
            texture = new(1, 1);
            return ImageConversion.LoadImage(texture, bytes);
        }
        private void BlockAsGridImageConversion()
        {
            if (referenceTexture.width % blockDimensions.x != 0)
            {
                Debug.LogError("Width is not divisible by dimension x");
                return;
            }
            if (referenceTexture.height % blockDimensions.y != 0)
            {
                Debug.LogError("Height is not divisible by dimension y");
                return;
            }
            for (int i = 0; i < referenceTexture.width / blockDimensions.x; i++)
            {
                for (int j = 0; j < referenceTexture.height / blockDimensions.y; j++)
                {
                    var texture = ReadBlockFromReferenceTexture(new(i * blockDimensions.x, j * blockDimensions.y));
                    //if the block exists, then put the position into the block.
                    //if not, add the block to the list
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

        private void BlockAsAnyPositionImageConversion()
        {
            for (int i = 0; i < referenceTexture.width - blockDimensions.x + 1; i++)
            {
                for (int j = 0; j < referenceTexture.height - blockDimensions.y + 1; j++)
                {
                    var texture = ReadBlockFromReferenceTexture(new(i, j));
                    //if the block exists, then put the position into the block.
                    //if not, add the block to the list
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

        public void UpdateReferences()
        {
            var cells = Resources.LoadAll<CellVariableSO>("CellVariables");
            var sprites = Resources.LoadAll<Sprite>("");
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].sprite = sprites[i];
            }
        }

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

        private Vector2Int[] GetNeighboursPosition(PositionedBlock block)
        {
            Vector2Int[] positions = new Vector2Int[block.positions.Count * 4];
            for (int i = 0; i < block.positions.Count; i++)
            {
                positions[4 * i + 0] = (new(GameManager.Mod(block.positions[i].x - 1, numberOfBlocksPerColoumn), block.positions[i].y));
                positions[4 * i + 1] = (new(GameManager.Mod(block.positions[i].x + 1, numberOfBlocksPerColoumn), block.positions[i].y));
                positions[4 * i + 2] = (new(block.positions[i].x, GameManager.Mod(block.positions[i].y - 1, numberOfBlocksPerLine)));
                positions[4 * i + 3] = (new(block.positions[i].x, GameManager.Mod(block.positions[i].y + 1, numberOfBlocksPerLine)));
            }

            return positions;
        }
        private void SetNeighbours(PositionedCellVariable positionedCellVariable, Vector2Int[] neighboursPositions)
        {
            HashSet<CellVariable>[] directionalNeighbours = new HashSet<CellVariable>[4];
            for (int i = 0; i < directionalNeighbours.Length; i++)
            {
                directionalNeighbours[i] = new();
            }
            for (int i = 0; i < positionedCellVariable.positions.Count; i++)
            {
                directionalNeighbours[0].Add(positionedCellVariables
                                                        .Where(x => HasPosition(x, neighboursPositions[0 + i * 4]))
                                                        .Select(x => x.cellVariable).First());
                directionalNeighbours[1].Add(positionedCellVariables
                                                            .Where(x => HasPosition(x, neighboursPositions[1 + i * 4]))
                                                            .Select(x => x.cellVariable).First());
                directionalNeighbours[2].Add(positionedCellVariables
                                                            .Where(x => HasPosition(x, neighboursPositions[2 + i * 4]))
                                                            .Select(x => x.cellVariable).First());
                directionalNeighbours[3].Add(positionedCellVariables
                                                            .Where(x => HasPosition(x, neighboursPositions[3 + i * 4]))
                                                            .Select(x => x.cellVariable).First());
            }
            positionedCellVariable.cellVariable.left = directionalNeighbours[0].ToList();
            positionedCellVariable.cellVariable.right = directionalNeighbours[1].ToList();
            positionedCellVariable.cellVariable.up = directionalNeighbours[2].ToList();
            positionedCellVariable.cellVariable.down = directionalNeighbours[3].ToList();
        }
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


    }
}
