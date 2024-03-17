using ProjectCore;
using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private float spawnInterval;

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }
    private void SpawnObstacle()
    {
        var obj = ProjectManager.RandomElement(obstacles);
        Instantiate(obj, transform.position, Quaternion.identity);
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
