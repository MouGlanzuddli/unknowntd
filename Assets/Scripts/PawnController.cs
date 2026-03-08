using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : BaseUnitController
{
    private PawnData pawnData;
    private Transform goldMine;

    protected override int IsMovingHash => Animator.StringToHash("isRunWithPickaxe");
    protected override int AttackTriggerHash => Animator.StringToHash("mine");

    public override void Init(UnitData unitData)
    {
        base.Init(unitData);
        pawnData = unitData as PawnData;

        if (pawnData != null)
        {
            attackCooldown = pawnData.mineCooldown;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (goldMine == null) return;

        if (!isAttacking)
        {
            if (aiPath.reachedEndOfPath)
            {
                TryMine();
            }
        }
    }

    protected override void UpdateTarget()
    {
        if (isAttacking) return;
        
        if (goldMine == null)
        {
            goldMine = GoldManager.Instance?.GetBestGold(this);
            SetTarget(goldMine);
        }
    }

    protected override bool IsTargetValid()
    {
        return goldMine != null;
    }

    private void TryMine()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;
        if (goldMine == null || !goldMine.CompareTag("Gold")) return;

        isAttacking = true;
        StopMove();
        animator.SetTrigger(AttackTriggerHash);
    }

    public override void AttackStart()
    {
        if (goldMine != null)
        {
            float dir = goldMine.position.x - transform.position.x;
            FlipTo(dir);
        }
    }

    public override void AttackHit()
    {
        PlayerResource.Instance.AddGold(1);
        // if (AudioManager.Instance != null) AudioManager.Instance.PlayRockHit(transform.position);
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
    }
}