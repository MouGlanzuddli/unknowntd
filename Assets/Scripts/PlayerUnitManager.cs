using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitManager : BaseSingleton<PlayerUnitManager>
{
    private List<BaseUnitController> activeUnits = new();
    public PlayerController Player { get; private set; }

    private Dictionary<UnitData, int> ownedCounts = new();
    private Dictionary<UnitData, int> spawnedCounts = new();

    public event Action<UnitData, int> OnUnitCountChanged;

    public void AddUnit(UnitData data)
    {
        if (!ownedCounts.ContainsKey(data))
            ownedCounts[data] = 0;

        ownedCounts[data]++;

        TriggerUpdate(data);
    }

    public bool CanSpawn(UnitData data)
    {
        return GetAvailableCount(data) > 0;
    }

    public void RegisterPlayer(PlayerController player)
    {
        Player = player;
        RegisterSpawn(player);
    }

    public void RegisterSpawn(BaseUnitController unit)
    {
        UnitData data = unit.Data;

        activeUnits.Add(unit);

        if (!spawnedCounts.ContainsKey(data))
            spawnedCounts[data] = 0;

        spawnedCounts[data]++;

        TriggerUpdate(data);
    }

    public void OnUnitDied(BaseUnitController unit)
    {
        UnitData data = unit.Data;

        activeUnits.Remove(unit);

        if (spawnedCounts.ContainsKey(data))
            spawnedCounts[data]--;

        if (ownedCounts.ContainsKey(data))
            ownedCounts[data]--;

        TriggerUpdate(data);
    }

    public int GetOwnedCount(UnitData data)
    {
        if (ownedCounts.TryGetValue(data, out int value))
            return value;

        return 0;
    }

    public int GetSpawnedCount(UnitData data)
    {
        if (spawnedCounts.TryGetValue(data, out int value))
            return value;

        return 0;
    }

    public int GetAvailableCount(UnitData data)
    {
        return GetOwnedCount(data) - GetSpawnedCount(data);
    }

    private void TriggerUpdate(UnitData data)
    {
        OnUnitCountChanged?.Invoke(data, GetAvailableCount(data));
    }

    public BaseUnitController GetClosestUnit(Vector3 position)
    {
        BaseUnitController closest = null;
        float minDist = float.MaxValue;

        foreach (var unit in activeUnits)
        {
            if (unit == null) continue;

            float dist = (unit.transform.position - position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = unit;
            }
        }

        return closest;
    }
}