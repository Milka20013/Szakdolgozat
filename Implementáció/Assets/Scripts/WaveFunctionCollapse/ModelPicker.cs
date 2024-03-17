using System;
using TMPro;
using UnityEngine;

namespace WFC
{
    public class ModelPicker : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private ImageGrid grid;
        private void Awake()
        {
            var options = Enum.GetNames(typeof(PickModel));
            dropdown.options.Clear();
            foreach (var item in options)
            {
                dropdown.options.Add(new(item));
            }
            dropdown.value = (int)grid.pickModel;
        }
    }
}
