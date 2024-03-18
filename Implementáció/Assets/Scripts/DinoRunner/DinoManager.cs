using UnityEngine;
using WFC;

public class DinoManager : MonoBehaviour
{

    [SerializeField] private TextureConverter converter;
    [SerializeField] private ImageGrid imageGrid;
    private void Start()
    {
        converter.Init();
        CollapseOptionManager.Instance.wildCard = CollapseOptionManager.Instance.cellVariables[0];
        imageGrid.Init();
    }
}
