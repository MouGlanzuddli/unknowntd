using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : BaseUnitController
{
    private CombatUnitData warriorData;

    public override void Init(UnitData unitData)
    {
        base.Init(unitData);
        warriorData = unitData as CombatUnitData;
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
        BaseUnitController enemy = FindEnemyTarget();

        if (enemy != null)
        {
            if (targetUnit != enemy)
            {
                SetTarget(enemy);
            }
        }
        else
        {
            var player = PlayerUnitManager.Instance.Player;
            if (player != null && targetUnit != player)
            {
                SetTarget(player, aggroRange);
            }
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;
        if (targetTransform == null && targetUnit == null) return;

        BaseUnitController target = targetUnit;
        if (target == null && targetTransform != null)
        {
            target = targetTransform.GetComponent<BaseUnitController>();
        }

        if (target == null || target.Data.faction != Faction.Enemy) return;

        if (!aiPath.reachedEndOfPath) return;

        isAttacking = true;
        animator.SetTrigger(AttackTriggerHash);
    }

    public override void AttackHit()
    {
        if (targetTransform == null && targetUnit == null)
            return;

        BaseUnitController unit = targetUnit;

        if (unit == null && targetTransform != null)
        {
            unit = targetTransform.GetComponent<BaseUnitController>();
        }

        if (unit == null)
            return;

        if (unit.Data.faction != Faction.Enemy)
            return;

        float distSqr = (GetTargetPosition() - transform.position).sqrMagnitude;
        float attackRangeSqr = attacktRange * attacktRange;
        if (distSqr > (attacktRange + 0.1f) * (attacktRange + 0.1f))
            return;

        unit.TakeDamage(data.damage);
        unit.SetTarget(this);
    }
}