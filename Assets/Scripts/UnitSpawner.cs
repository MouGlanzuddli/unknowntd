using UnityEngine;
using System.Collections;

public class UnitSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float spawnInterval;
    }

    [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        while (currentWaveIndex < waves.Length)
        {
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves finished!");
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isSpawning = false;
    }
}