using System.Collections.Generic;
using UnityEngine;

public class EndlessTrackSpawner : MonoBehaviour
{
    [Header("Track Settings")]
    [SerializeField] private GameObject trackSegmentPrefab;
    [SerializeField] private int segmentCount = 5;
    [SerializeField] private float segmentLength = 20f;

    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private float[] laneXPositions = { -3f, 0f, 3f };
    [Range(0, 3)]
    [SerializeField] private int minObstaclesPerSegment = 0;
    [Range(0, 3)]
    [SerializeField] private int maxObstaclesPerSegment = 1;
    [Range(0f, 1f)]
    [SerializeField] private float obstacleSpawnChance = 0.7f;

    [Header("References")]
    [SerializeField] private Transform player;

    private List<GameObject> activeSegments = new List<GameObject>();
    private List<List<GameObject>> segmentObstacles = new List<List<GameObject>>();
    private float nextSpawnZ = 0f;

    private void Start()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            SpawnSegment();
        }
    }

    private void Update()
    {
        if (player == null || activeSegments.Count == 0)
        {
            return;
        }

        GameObject firstSegment = activeSegments[0];

        if (player.position.z - firstSegment.transform.position.z > segmentLength)
        {
            RecycleSegment();
        }
    }

    private void SpawnSegment()
    {
        GameObject segment = Instantiate(trackSegmentPrefab, new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
        activeSegments.Add(segment);

        List<GameObject> obstacles = SpawnObstaclesOnSegment(nextSpawnZ);
        segmentObstacles.Add(obstacles);

        nextSpawnZ += segmentLength;
    }

    private void RecycleSegment()
    {
        GameObject segment = activeSegments[0];
        activeSegments.RemoveAt(0);

        List<GameObject> oldObstacles = segmentObstacles[0];
        segmentObstacles.RemoveAt(0);
        foreach (GameObject obstacle in oldObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }

        segment.transform.position = new Vector3(0f, 0f, nextSpawnZ);
        activeSegments.Add(segment);

        List<GameObject> newObstacles = SpawnObstaclesOnSegment(nextSpawnZ);
        segmentObstacles.Add(newObstacles);

        nextSpawnZ += segmentLength;
    }

    private List<GameObject> SpawnObstaclesOnSegment(float segmentZ)
    {
        List<GameObject> spawned = new List<GameObject>();

        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            return spawned;
        }

        if (Random.value > obstacleSpawnChance)
        {
            return spawned;
        }

        int count = Random.Range(minObstaclesPerSegment, maxObstaclesPerSegment + 1);
        List<float> availableLanes = new List<float>(laneXPositions);

        for (int i = 0; i < count && availableLanes.Count > 0; i++)
        {
            int laneIndex = Random.Range(0, availableLanes.Count);
            float laneX = availableLanes[laneIndex];
            availableLanes.RemoveAt(laneIndex);

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            float spawnZ = segmentZ + Random.Range(segmentLength * 0.2f, segmentLength * 0.8f);

            GameObject obstacle = Instantiate(prefab, new Vector3(laneX, 0f, spawnZ), Quaternion.identity);
            spawned.Add(obstacle);
        }

        return spawned;
    }
}