using ProjectCore;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;
    public bool autoUpdateEditor;

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainData terrainData;

    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        var map = Noise.GeneratePerlinNoiseMap(mapWidth, mapHeight, noiseScale);
        terrainData.SetHeights(0, 0, map);
        terrain.Flush();
    }
}
