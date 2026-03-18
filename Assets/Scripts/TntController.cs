using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TntController : BaseUnitController
{
    [Header("Tnt Settings")]
    [SerializeField] private GameObject tntPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float explosionRadius = 1.5f;

    private CombatUnitData tntData;

    public override void Init(UnitData unitData)
    {
        base.Init(unitData);
        tntData = unitData as CombatUnitData;
    }

    protected override void Update()
    {
        base.Update();

        if (!isAttacking)
        {
            if (aiPath.reachedEndOfPath)
            {
                TryAttack();
            }
        }
    }

    protected override void UpdateTarget()
    {
        if (isAttacking) return;

        BaseUnitController player = PlayerUnitManager.Instance.GetClosestUnit(transform.position);

        if (player != null)
        {
            if (targetUnit != player)
            {
                SetTarget(player);
            }
            return;
        }

        if (targetTransform == null || targetTransform != Castle.Instance.transform)
        {
            SetTarget(Castle.Instance.transform);
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;
        if (targetTransform == null) return;
        if (!aiPath.reachedEndOfPath) return;

        isAttacking = true;
        animator.SetTrigger(AttackTriggerHash);
    }

    public override void AttackHit()
    {
        if (targetTransform == null) return;
        if (tntPrefab == null)
        {
            Debug.LogWarning("Tnt Prefab is not assigned in TntController!");
            return;
        }

        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;
        GameObject tntObj = Instantiate(tntPrefab, spawnPos, Quaternion.identity);
        
        Tnt tnt = tntObj.GetComponent<Tnt>();
        if (tnt != null)
        {
            tnt.Init(data.damage, GetTargetPosition(), explosionRadius);
        }
    }
}
