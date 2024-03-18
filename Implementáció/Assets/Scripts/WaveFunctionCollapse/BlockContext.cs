using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WFC
{
    /// <summary>
    /// UI component, this is the popup window after "check blocks" are clicked
    /// </summary>
    public class BlockContext : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform container;
        private List<GameObject> spawnedObjects = new();
        public void Show(List<PositionedCellVariable> cellVariables)
        {
            int diff = cellVariables.Count - spawnedObjects.Count;
            for (int i = 0; i < diff; i++)
            {
                var cell = Instantiate(cellPrefab, container);
                spawnedObjects.Add(cell);
            }
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                var cellContext = spawnedObjects[i].GetComponent<BlockContextCell>();
                cellContext.blockContext = this;
                cellContext.cellVariable = cellVariables[i].cellVariable;
                spawnedObjects[i].GetComponentInChildren<Image>().sprite = cellVariables[i].cellVariable.sprite;
                spawnedObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = cellVariables[i].cellVariable.GetWeight().ToString();
            }
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void ChangeWeight(string message, CellVariable cellVariable)
        {
            if (float.TryParse(message, out float result))
            {
                if (result >= 0f)
                {
                    cellVariable.weight = result;
                }
                else
                {
                    CollapseOptionManager.Instance.wildCard = cellVariable;
                }
            }
        }
    }
}
