using UnityEditor;
using UnityEngine;
using WFC;

[CustomEditor(typeof(ImageGrid))]
public class ImageGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var imageGrid = (ImageGrid)target;
        /*if (GUILayout.Button("Create Cells"))
        {
            inputReader.CreateCells();
        }*/
        if (GUILayout.Button("Export image"))
        {
            imageGrid.ExportImage();
        }
    }
}
