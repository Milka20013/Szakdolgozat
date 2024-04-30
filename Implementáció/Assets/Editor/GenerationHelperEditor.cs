using UnityEditor;
using UnityEngine;
using WFC;

[CustomEditor(typeof(GenerationHelper))]
public class GenerationHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Count"))
        {
            ((GenerationHelper)target).CountBlocks();
        }
    }
}
