using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace WFC
{
    /// <summary>
    /// Wrapper class to assign position and occurance to a texture block.
    /// </summary>
    public class PositionedBlock
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
}
