using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : BaseSingleton<Castle>, IDamageable
{
    [SerializeField] private Transform gate;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;

    protected Health health;

    protected override void Awake()
    {
        base.Awake();
        gate = transform.Find("Gate");

    }

    private void Start()
    {
        InitHealth();
    }
    protected virtual void InitHealth()
    {
        health = new Health(maxHealth, healthBar);
        health.OnDeath += GameManager.Instance.LoseGame;
    }
    public void SpawnUnit(UnitData data)
    {
        if (!PlayerUnitManager.Instance.CanSpawn(data))
            return;

        Vector3 spawnPos = gate.position;

        GameObject obj = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        BaseUnitController unit = obj.GetComponent<BaseUnitController>();
        unit.Init(data);

        PlayerUnitManager.Instance.RegisterSpawn(unit);
    }

    public void TakeDamage(float amount)
    {
        health.Damage(amount);
    }
}