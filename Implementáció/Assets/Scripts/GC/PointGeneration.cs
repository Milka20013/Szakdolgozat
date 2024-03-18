using ProjectCore;
using System.Collections.Generic;
using UnityEngine;


namespace GC
{
    public class PointGeneration : MonoBehaviour
    {
        [SerializeField] private CelestialBodySOContainer container;
        public int maxNumberOfStars;
        public int maxNumberOfBlackHoles;
        public float blackHoleClearZoneRadius = 1f;
        private Point[,] points;
        [Range(0f, 1f)]
        public float resourceDensity;
        public Vector2Int dimension;
        public float spacing;
        public float positionNoise;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject resourcePrefab;

        private List<GameObject> generatedObjects = new();
        /// <summary>
        /// Run the generation
        /// </summary>
        public void RunAlgorithm()
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
        /// <summary>
        /// Generate maxNumberOfStars amount of stars randomly in a grid given by the dimension variable.
        /// </summary>
        /// <returns></returns>
        private int[] GenerateStars()
        {
            int[] indexArray = new int[dimension.x * dimension.y];
            for (int i = 0; i < indexArray.Length; i++)
            {
                indexArray[i] = i;
            }
            ProjectManager.ShuffleArray(indexArray);
            for (int i = 0; i < maxNumberOfStars; i++)
            {
                var x = indexArray[i] / dimension.y;
                var y = indexArray[i] % dimension.y;
                points[x, y].SetType(PointType.Star);
                points[x, y].RandomizePosition(positionNoise / 2);
            }
            return indexArray;
        }
        /// <summary>
        /// Generate a maximum number of maxNumberOfBlackHoles black holes. The holes are generated where the stars did not.
        /// A black hole rules out other black holes in a 5x5 area, and nothing can be next to the black hole in a 3x3 area.
        /// Execption to this are the stars that are farther away from the hole than the blackHoleClearZoneRadius units.
        /// </summary>
        /// <param name="indexArray"></param>
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
                EliminateNeighbors(x, y, 3, 3, points, PointType.Any, PointType.None, blackHoleClearZoneRadius);

            }
            EliminateTemporaries();
        }
        /// <summary>
        /// Generate resources where the points can be anything. The density is controlled by the resourceDensity variable.
        /// </summary>
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
        /// <summary>
        /// Random point generation to show the difference
        /// Might be used in presentation
        /// </summary>
        public void GeneratePoints2()
        {
            points = new Point[1, maxNumberOfStars];
            for (int i = 0; i < maxNumberOfStars; i++)
            {
                points[0, i] = new(Random.Range(0f, dimension.x), Random.Range(0f, dimension.y), PointType.Planet);
            }
        }

        /// <summary>
        /// Set the neighboring cells type into an other based on the type and position of the cell.
        /// The algorithm considers the distance from the original cell as well.
        /// </summary>
        /// <param name="x">x coordinate of the initial cell</param>
        /// <param name="y">y coordinate of the initial cell</param>
        /// <param name="width">Width of the area to check neighbors in</param>
        /// <param name="height">Height of the area to check neighbors in</param>
        /// <param name="array">The point array to iterate to. May be removed</param>
        /// <param name="targetType">These types should be eliminated. If set to any, any type can be eliminated</param>
        /// <param name="elimType">The points will have this type</param>
        /// <param name="minDistance">If the points are farther away than this parameter, they don't get removed</param>
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
        /// <summary>
        /// Set every temporary point to unknown
        /// </summary>
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
            RunAlgorithm();
            InstantiatePoints();
        }
        /// <summary>
        /// Run the algorithm normally
        /// Used in editor
        /// </summary>
        public void GeneratePseudoRandomPoints()
        {
            Clear();
            RunAlgorithm();
            InstantiatePoints();
        }
        /// <summary>
        /// Used in editor
        /// TO-DO: Clear this up
        /// </summary>
        public void GenerateRandomRandomPoints()
        {
            Clear();
            GeneratePoints2();
            InstantiatePoints();

        }
        /// <summary>
        /// Physically spawn the points into the scene
        /// </summary>
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
        /// <summary>
        /// Get the prefab to spawn from the Scriptable Object container.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private GameObject GetPrefab(Point point)
        {
            return container.GetPrefab(point);
        }
        /// <summary>
        /// Deestroy every spawned object
        /// </summary>
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
