using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Enemy
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject[] enemyPrefabs;
        public int[] enemyCounts;
        public float spawnInterval = 1f;
        public float timeBetweenWaves = 5f;
    }

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Wave[] waves;
        [SerializeField] private bool autoStartWaves = true;
        [SerializeField] private float firstWaveDelay = 2f;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;

        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private int totalEnemiesAlive = 0;

        public int CurrentWave => currentWaveIndex;
        public int TotalWaves => waves.Length;
        public bool IsSpawning => isSpawning;

        private void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            if (autoStartWaves)
            {
                StartCoroutine(StartWavesWithDelay());
            }
        }

        private IEnumerator StartWavesWithDelay()
        {
            yield return new WaitForSeconds(firstWaveDelay);
            StartNextWave();
        }

        public void StartNextWave()
        {
            if (isSpawning) return;
            if (currentWaveIndex >= waves.Length)
            {
                Debug.Log("All waves completed!");
                OnAllWavesCompleted();
                return;
            }

            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }

        private IEnumerator SpawnWave(Wave wave)
        {
            isSpawning = true;

            if (showDebugInfo)
            {
                Debug.Log($"Starting {wave.waveName}");
            }

            // Notify GameManager
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnWaveStart(currentWaveIndex);
            }

            // Spawn all enemies in wave
            for (int i = 0; i < wave.enemyPrefabs.Length; i++)
            {
                if (wave.enemyPrefabs[i] == null) continue;

                int count = i < wave.enemyCounts.Length ? wave.enemyCounts[i] : 1;

                for (int j = 0; j < count; j++)
                {
                    SpawnEnemy(wave.enemyPrefabs[i]);
                    yield return new WaitForSeconds(wave.spawnInterval);
                }
            }

            isSpawning = false;
            currentWaveIndex++;

            // Wait for all enemies to be defeated before next wave
            yield return new WaitUntil(() => totalEnemiesAlive == 0);

            if (showDebugInfo)
            {
                Debug.Log($"{wave.waveName} completed!");
            }

            // Notify GameManager
            if (gameManager != null)
            {
                gameManager.OnWaveComplete(currentWaveIndex - 1);
            }

            // Wait before next wave
            if (currentWaveIndex < waves.Length)
            {
                yield return new WaitForSeconds(wave.timeBetweenWaves);
                StartNextWave();
            }
            else
            {
                OnAllWavesCompleted();
            }
        }

        private void SpawnEnemy(GameObject enemyPrefab)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            totalEnemiesAlive++;

            // Subscribe to enemy death
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                StartCoroutine(WaitForEnemyDeath(enemy));
            }
        }

        private IEnumerator WaitForEnemyDeath(GameObject enemy)
        {
            yield return new WaitUntil(() => enemy == null);
            totalEnemiesAlive--;
        }

        private void OnAllWavesCompleted()
        {
            if (showDebugInfo)
            {
                Debug.Log("All waves completed! Victory!");
            }

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnAllWavesCompleted();
            }
        }

        // Manual control
        public void ForceStartNextWave()
        {
            if (!isSpawning && currentWaveIndex < waves.Length)
            {
                StopAllCoroutines();
                StartNextWave();
            }
        }

        private void OnDrawGizmos()
        {
            if (spawnPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.right * 1f);
        }
    }
}
