using UnityEngine;

namespace WFC
{
    /// <summary>
    /// UI component, cells used in block context
    /// </summary>
    public class BlockContextCell : MonoBehaviour
    {
        public BlockContext blockContext;
        public CellVariable cellVariable;

        public void OnInputEnd(string message)
        {
            blockContext.ChangeWeight(message, cellVariable);
        }
    }
}
