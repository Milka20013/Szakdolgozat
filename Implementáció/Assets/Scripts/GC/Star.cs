using System.Collections.Generic;
using UnityEngine;

namespace GC
{
    public class Star : CelestialBody
    {
        [SerializeField] private GameObject planetPrefab;
        [Range(0f, 1f)]
        [SerializeField] private float planetDensity;
        [SerializeField] private int maxNumberOfPlanets;
        private List<GameObject> planets = new();
        private void Start()
        {
            planetDensity = PointGeneration.instance.planetDensity;
            maxNumberOfPlanets = PointGeneration.instance.maxPlanetsPerStar;
            SpawnPlanets();
        }
        private void SpawnPlanets()
        {
            int currentPlanetAmount = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (currentPlanetAmount >= maxNumberOfPlanets)
                    {
                        return;
                    }
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    if (Random.Range(0f, 1f) > planetDensity)
                    {
                        continue;
                    }
                    currentPlanetAmount++;
                    var pos = new Vector3(transform.position.x + i, transform.position.y + j);
                    var planet = Instantiate(planetPrefab, pos, Quaternion.identity);
                    planet.GetComponent<Planet>().star = this;
                    planets.Add(planet);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in planets)
            {
                Destroy(item);
            }
        }
    }
}
