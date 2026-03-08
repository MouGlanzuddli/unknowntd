using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : BaseUnitController
{
    private CombatUnitData torchData;

    public override void Init(UnitData unitData)
    {
        base.Init(unitData);
        torchData = unitData as CombatUnitData;
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

        // Nếu không có player unit, tấn công Castle
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

        float distSqr = (GetTargetPosition() - transform.position).sqrMagnitude;
        float attackRangeWithBuffer = attacktRange + 0.1f;
        if (distSqr > attackRangeWithBuffer * attackRangeWithBuffer) 
            return;

        Castle castle = targetTransform.GetComponent<Castle>();
        if (castle == null && targetTransform == Castle.Instance.transform)
        {
            castle = Castle.Instance;
        }

        if (castle != null)
        {
            castle.TakeDamage(data.damage);
            return;
        }

        IDamageable damageable = targetTransform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(data.damage);
        }
    }
}