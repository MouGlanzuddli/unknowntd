using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : BaseSingleton<LevelManager>
{
    [Header("Levels")]
    public List<LevelData> levels = new();

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;

    public string CurrentLevelName => levels.Count > 0 ? levels[Mathf.Clamp(currentLevelIndex, 0, levels.Count - 1)].levelName : "No Level";
    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWavesInLevel => levels.Count > 0 ? levels[Mathf.Clamp(currentLevelIndex, 0, levels.Count - 1)].waves.Count : 0;
    public bool IsAllLevelsCompleted => currentLevelIndex >= levels.Count;

    public int TotalWavesAcrossAllLevels
    {
        get
        {
            int total = 0;
            foreach (var level in levels)
            {
                total += level.waves.Count;
            }
            return total;
        }
    }

    public int TotalWavesCompleted
    {
        get
        {
            int completed = 0;
            for (int i = 0; i < currentLevelIndex && i < levels.Count; i++)
            {
                completed += levels[i].waves.Count;
            }
            
            if (currentLevelIndex < levels.Count)
            {
                completed += currentWaveIndex;
            }
            return completed;
        }
    }

    public WaveData GetCurrentWave()
    {
        if (currentLevelIndex >= levels.Count) return null;
        return levels[currentLevelIndex].waves[currentWaveIndex];
    }

    public bool NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= levels[currentLevelIndex].waves.Count)
        {
            currentWaveIndex = 0;
            currentLevelIndex++;

            if (currentLevelIndex >= levels.Count)
                return false;
        }

        OnWaveChanged?.Invoke();
        return true;
    }

    public static event System.Action OnWaveChanged;

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public List<WaveData> waves = new();
    }

    [System.Serializable]
    public class WaveData
    {
        public int enemyCount;
        public float startDelay = 30f;
        public float spawnInterval = 0.5f;
        public List<CombatUnitData> enemies = new();
    }
}