using ProjectCore;
using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private float spawnInterval;
    private int numberOfSpawnedObjects = 0;
    private string[] sceneNames = { "GC", "WFCSelectorScene", "SubwaySurfer" };

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }
    private void SpawnObstacle()
    {
        var obj = ProjectManager.RandomElement(obstacles);
        var instance = Instantiate(obj, transform.position, Quaternion.identity);
        var obstacle = instance.GetComponent<Obstacle>();
        obstacle.sceneName = sceneNames[numberOfSpawnedObjects % sceneNames.Length];
        obstacle.text.text = obstacle.sceneName;
        numberOfSpawnedObjects++;
    }

    IEnumerator SpawnObjects()
    {
        for (; ; )
        {
            SpawnObstacle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
