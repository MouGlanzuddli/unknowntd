using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnUnit
    {
        public GameObject prefab;
        public int amount;
    }

    [System.Serializable]
    public class Wave
    {
        public List<SpawnUnit> units;
        public float spawnRate = 1f;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;

    [Header("General Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("UI Animation Settings")]
    [SerializeField] private WaveBannerController waveBanner;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    //private int enemiesAlive = 0; // ADDED

    private void Start()
    {
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Count && GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            UpdateWaveUI();
            if (waveBanner != null)
            {
                waveBanner.PlayWaveAnimation();
            }
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            //yield return new WaitUntil(() => enemiesAlive == 0);

            currentWaveIndex++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves completed!");
        GameStateManager.Instance.SetState(GameState.Victory);
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        foreach (SpawnUnit unit in wave.units)
        {
            for (int i = 0; i < unit.amount; i++)
            {
                GameObject enemy = Instantiate(unit.prefab, spawnPoint.position, Quaternion.identity);

                //enemiesAlive++; // ADDED

                //Enemy enemyScript = enemy.GetComponent<Enemy>(); // ADDED
                //if (enemyScript != null)
                //{
                //    enemyScript.spawner = this; // ADDED
                //}

                yield return new WaitForSeconds(wave.spawnRate);
            }
        }

        isSpawning = false;
    }

    private void UpdateWaveUI()
    {
        waveText.text = "Wave " + (currentWaveIndex + 1) + " / " + waves.Count;
    }

    //// ADDED: usage: void Die() in Enemy.cs
    //{
    //    if (spawner != null)
    //    {
    //        spawner.EnemyDied(); // ADDED
    //    }

    //Destroy(gameObject);
    //}
    //public void EnemyDied()
    //{
    //    enemiesAlive--;
    //}
}