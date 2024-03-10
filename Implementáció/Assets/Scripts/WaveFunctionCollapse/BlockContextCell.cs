using UnityEngine;

namespace WFC
{
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
