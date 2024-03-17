using ProjectCore;
using System.Collections.Generic;
using UnityEngine;


namespace GC
{
    public class PointGeneration : MonoBehaviour
    {
        [SerializeField] private CelestialBodySOContainer container;
        [SerializeField] private int maxNumberOfStars;
        [SerializeField] private int maxNumberOfBlackHoles;
        private Point[,] points;
        [Range(0f, 1f)]
        [SerializeField] private float resourceDensity;
        [SerializeField] private Vector2Int dimension;
        [SerializeField] private float spacing;
        [SerializeField] private float positionNoise;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject resourcePrefab;

        private List<GameObject> generatedObjects = new();

        public void GeneratePoints()
        {
            points = new Point[dimension.x, dimension.y];

            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    points[i, j] = new(i, j, PointType.Unknown);
                }
            }


            var indexArray = GenerateStars();

            GenerateBlackHoles(indexArray);

            GenerateResources();
        }

        private int[] GenerateStars()
        {
            int[] indexArray = new int[dimension.x * dimension.y];
            for (int i = 0; i < indexArray.Length; i++)
            {
                indexArray[i] = i;
            }
            ProjectManager.ShuffleArray(indexArray);
            for (int i = 0; i < maxNumberOfStars * 2; i++)
            {
                var x = indexArray[i] / dimension.y;
                var y = indexArray[i] % dimension.y;
                points[x, y].SetType(PointType.Star);
                points[x, y].RandomizePosition(positionNoise / 2);
            }
            return indexArray;
        }

        private void GenerateBlackHoles(int[] indexArray)
        {
            int currentBlackHoleNumber = 0;
            for (int i = maxNumberOfStars - 1; i < indexArray.Length; i++)
            {
                if (currentBlackHoleNumber >= maxNumberOfBlackHoles)
                {
                    break;
                }
                var x = indexArray[i] / dimension.y;
                var y = indexArray[i] % dimension.y;
                if (points[x, y].type != PointType.Unknown)
                {
                    continue;
                }
                currentBlackHoleNumber++;
                points[x, y].SetType(PointType.Black_Hole);
                EliminateNeighbors(x, y, 5, 5, points, PointType.Unknown, PointType.Temporary, 50);
                points[x, y].RandomizePosition(positionNoise / 2);
                EliminateNeighbors(x, y, 3, 3, points, PointType.Any, PointType.None, 1);

            }
            EliminateTemporaries();
        }

        private void GenerateResources()
        {
            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    if (points[i, j].type == PointType.Unknown)
                    {
                        if (Random.Range(0f, 1f) <= resourceDensity)
                        {
                            points[i, j].type = PointType.Resource;
                            points[i, j].RandomizePosition(positionNoise / 3);

                        }
                    }
                }
            }
        }
        public void GeneratePoints2()
        {
            points = new Point[1, maxNumberOfStars];
            for (int i = 0; i < maxNumberOfStars; i++)
            {
                points[0, i] = new(Random.Range(0f, dimension.x), Random.Range(0f, dimension.y), PointType.Planet);
            }
        }


        private void EliminateNeighbors(int x, int y, int width, int height, Point[,] array,
            PointType targetType, PointType elimType, float minDistance)
        {
            int newWidth = (width - 1) / 2;
            int newHeight = (height - 1) / 2;
            for (int i = Mathf.Clamp(x - newWidth, 0, dimension.x); i <= Mathf.Clamp(x + newWidth, 0, dimension.x - 1); i++)
            {
                for (int j = Mathf.Clamp(y - newHeight, 0, dimension.y); j <= Mathf.Clamp(y + newHeight, 0, dimension.y - 1); j++)
                {
                    if (i == x && j == y)
                    {
                        continue;
                    }
                    if (targetType != PointType.Any)
                    {
                        if (array[i, j].type != targetType)
                        {
                            continue;
                        }
                    }
                    if (array[i, j].Distance(array[x, y]) >= minDistance)
                    {
                        continue;
                    }
                    array[i, j].type = elimType;
                }
            }
        }

        private void EliminateTemporaries()
        {
            for (int i = 0; i < dimension.x; i++)
            {
                for (int j = 0; j < dimension.y; j++)
                {
                    if (points[i, j].type == PointType.Temporary)
                    {
                        points[i, j].type = PointType.Unknown;
                    }
                }
            }
        }
        private void Start()
        {
            GeneratePoints();
            InstantiatePoints();
        }

        public void GeneratePseudoRandomPoints()
        {
            Clear();
            GeneratePoints();
            InstantiatePoints();
        }
        public void GenerateRandomRandomPoints()
        {
            Clear();
            GeneratePoints2();
            InstantiatePoints();

        }

        private void InstantiatePoints()
        {
            foreach (var item in points)
            {
                var prefab = GetPrefab(item);
                //Debug.Log(item.type);
                if (prefab == null)
                {
                    continue;
                }
                var pos = new Vector3(transform.position.x + item.position.x, transform.position.y + item.position.y);
                var instance = Instantiate(prefab, pos, Quaternion.identity);
                generatedObjects.Add(instance);
                instance.GetComponent<CelestialBody>().Init(item, new(dimension.x, dimension.y), spacing);
            }
        }

        private GameObject GetPrefab(Point point)
        {
            return container.GetPrefab(point);
        }

        private void Clear()
        {
            for (int i = 0; i < generatedObjects.Count; i++)
            {
                Destroy(generatedObjects[i]);
                generatedObjects[i] = null;
            }
            generatedObjects.RemoveAll(x => x == null);
        }
    }
}
