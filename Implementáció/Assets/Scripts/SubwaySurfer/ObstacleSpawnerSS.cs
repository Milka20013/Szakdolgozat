using ProjectCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubwaySurfer
{
    public class ObstacleSpawnerSS : MonoBehaviour
    {
        [SerializeField] private GameObject obstacle;
        [SerializeField] private float spawnInterval = 2f;
        [Range(0f, 1f)]
        [SerializeField] private float spawnChance = 0.5f;


        private void Start()
        {
            StartCoroutine(SpawnObstacles());
        }
        private void InstantiateObjects()
        {
            List<GameObject> objects = new();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (Random.Range(0f, 1f) > spawnChance)
                    {
                        continue;
                    }
                    objects.Add(Instantiate(obstacle, new Vector3(i, j, transform.position.z), Quaternion.identity));
                }
            }
            if (objects.Count == 9)
            {
                var objectArray = objects.ToArray();
                ProjectManager.ShuffleArray(objectArray);
                Destroy(objectArray[0]);
            }
        }

        IEnumerator SpawnObstacles()
        {
            for (; ; )
            {
                InstantiateObjects();
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
