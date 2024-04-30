using UnityEditor;
using UnityEngine;
namespace GC
{
    [CustomEditor(typeof(PointGeneration))]
    public class PointGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var gen = (PointGeneration)target;
            if (GUILayout.Button("GenerateRandomRandomPoints"))
            {
                gen.GenerateRandomRandomPoints();
            }
            if (GUILayout.Button("GeneratePseudoRandomPoints"))
            {
                gen.GeneratePseudoRandomPoints();
            }
            if (GUILayout.Button("Count"))
            {
                gen.CountObjects();
            }
        }
    }
}
