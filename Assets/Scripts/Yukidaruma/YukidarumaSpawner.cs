using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukidarumaSpawner : MonoBehaviour
{
    [SerializeField] private GameObject snowSpiritPrefab; // 雪霊プレハブ
    [SerializeField] private List<Transform> spawnPoints; // スポーン地点リスト

    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 10f;

    [SerializeField] private int maxSpawnCount = 10;
    private int currentSpawned = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (currentSpawned < maxSpawnCount)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            SpawnSnowSpirit();
        }
    }

    private void SpawnSnowSpirit()
    {
        if (spawnPoints.Count == 0 || snowSpiritPrefab == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(snowSpiritPrefab, spawnPoint.position, spawnPoint.rotation);
        currentSpawned++;
    }
}
