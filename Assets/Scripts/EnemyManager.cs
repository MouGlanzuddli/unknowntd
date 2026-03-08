using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : BaseSingleton<EnemyManager>
{

    private readonly List<BaseUnitController> enemies = new();

    public IReadOnlyList<BaseUnitController> Enemies => enemies;

    public void Register(BaseUnitController enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(BaseUnitController enemy)
    {
        enemies.Remove(enemy);
    }

    public BaseUnitController GetClosestEnemy(Vector3 position)
    {
        BaseUnitController closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = (enemy.transform.position - position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}