using UnityEditor;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        var manager = (TerrainManager)target;
        if (DrawDefaultInspector())
        {
            if (manager.autoUpdateEditor)
            {
                manager.GenerateMap();
            }
        }
    }
}
