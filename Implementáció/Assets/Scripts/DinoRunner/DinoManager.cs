using System.Collections;
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
        imageGrid.onAlgorithmEnd.AddListener(LogAsd);
        //StartCoroutine(LateStart());
    }

    public void LogAsd()
    {
        Debug.Log("ASD");
    }
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1f);
        imageGrid.Init();
    }
}
