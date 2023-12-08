using UnityEditor;
using UnityEngine;
using WFC;

[CustomEditor(typeof(InputReader))]
public class InputReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var inputReader = (InputReader)target;
        if (GUILayout.Button("Create Cells"))
        {
            inputReader.CreateCells();
        }
        if (GUILayout.Button("Update References"))
        {
            inputReader.UpdateReferences();
        }
    }
}
