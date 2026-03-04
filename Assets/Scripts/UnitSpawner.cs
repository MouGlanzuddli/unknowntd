using UnityEngine;
using System.Collections.Generic;
using Unit;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool autoSpawn = false;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxUnits = 10;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private float timer;
    private List<GameObject> spawnedUnits = new List<GameObject>();
    private GameManager gameManager;

    public int CurrentUnitCount => spawnedUnits.Count;
    public int MaxUnits => maxUnits;
    public bool CanSpawn => spawnedUnits.Count < maxUnits;

    private void Start()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!autoSpawn) return;
        if (unitPrefab == null || spawnPoint == null) return;
        if (spawnedUnits.Count >= maxUnits) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnUnit();
            timer = 0f;
        }
    }

    // ======================
    // SPAWN METHODS
    // ======================
    
    /// <summary>
    /// Try to spawn a unit (checks gold cost)
    /// </summary>
    public bool TrySpawnUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned!");
            return false;
        }

        if (spawnedUnits.Count >= maxUnits)
        {
            if (showDebugInfo)
                Debug.Log("Cannot spawn: Max units reached!");
            return false;
        }

        // Get unit cost
        UnitCombat unitCombat = unitPrefab.GetComponent<UnitCombat>();
        int cost = unitCombat != null ? unitCombat.Cost : 0;

        // Check if player has enough gold
        if (gameManager != null && cost > 0)
        {
            if (!gameManager.SpendGold(cost))
            {
                if (showDebugInfo)
                    Debug.Log($"Not enough gold! Need {cost}, have {gameManager.Gold}");
                return false;
            }
        }

        // Spawn unit
        SpawnUnit();
        return true;
    }

    /// <summary>
    /// Force spawn unit without cost check (for testing)
    /// </summary>
    public void ForceSpawnUnit()
    {
        if (spawnedUnits.Count >= maxUnits)
        {
            Debug.LogWarning("Cannot spawn: Max units reached!");
            return;
        }

        SpawnUnit();
    }

    private void SpawnUnit()
    {
        GameObject unit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        spawnedUnits.Add(unit);

        if (showDebugInfo)
        {
            Debug.Log($"Unit spawned! ({spawnedUnits.Count}/{maxUnits})");
        }
    }

    // ======================
    // UNIT TRACKING
    // ======================
    
    public void NotifyUnitDestroyed()
    {
        // Clean up null references
        spawnedUnits.RemoveAll(unit => unit == null);

        if (showDebugInfo)
        {
            Debug.Log($"Unit destroyed. Remaining: {spawnedUnits.Count}/{maxUnits}");
        }
    }

    // ======================
    // UTILITY
    // ======================
    
    public void SetMaxUnits(int max)
    {
        maxUnits = Mathf.Max(1, max);
    }

    public void ClearAllUnits()
    {
        foreach (var unit in spawnedUnits)
        {
            if (unit != null)
                Destroy(unit);
        }
        spawnedUnits.Clear();
    }

    // ======================
    // GIZMOS
    // ======================
    
    private void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 1f);
    }
}
