using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new();

    private int aliveEnemies;
    private bool spawning;

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        var wave = LevelManager.Instance.GetCurrentWave();
        if (wave == null) yield break;

        spawning = true;

        float countdown = wave.startDelay;
        while (countdown > 0)
        {
            if (GameUI.Instance != null)
                GameUI.Instance.UpdateCountdown(countdown);
                
            yield return new WaitForSeconds(0.05f);
            countdown -= 0.05f;
        }

        if (GameUI.Instance != null)
            GameUI.Instance.UpdateCountdown(0);

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        spawning = false;
    }

    void SpawnEnemy(LevelManager.WaveData wave)
    {
        int enemyIndex = Random.Range(0, wave.enemies.Count);
        CombatUnitData enemyData = wave.enemies[enemyIndex];

        int spawnIndex = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[spawnIndex];

        var enemy = Instantiate(enemyData.prefab, spawnPoint.position, Quaternion.identity);
        BaseUnitController baseUnitController = enemy.GetComponent<BaseUnitController>();
        baseUnitController.Init(enemyData);
        baseUnitController.SetTarget(Castle.Instance.transform);
        baseUnitController.AddDeathListener(OnEnemyDeath);

        EnemyManager.Instance.Register(baseUnitController);

        aliveEnemies++;
    }

    void OnEnemyDeath(BaseUnitController unit)
    {
        aliveEnemies--;

        if (!spawning && aliveEnemies <= 0)
        {
            HandleWaveClear();
        }
    }

    void HandleWaveClear()
    {
        bool hasNext = LevelManager.Instance.NextWave();

        if (!hasNext)
        {
            GameManager.Instance.WinGame();
            return;
        }

        StartCoroutine(StartWave());
    }
}