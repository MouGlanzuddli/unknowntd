using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvents : MonoBehaviour
{
    public event Action OnMineStart;
    public event Action OnMineHit;
    public event Action OnMineEnd;

    public event Action OnAttackStart;
    public event Action OnAttackHit;
    public event Action OnAttackEnd;

    public event Action OnHitStart;
    public event Action OnHit;
    public event Action OnHitEnd;

    public void Anim_MineStart()
    {
        OnMineStart?.Invoke();
    }

    public void Anim_MineHit()
    {
        OnMineHit?.Invoke();
    }

    public void Anim_MineEnd()
    {
        OnMineEnd?.Invoke();
    }

    public void Anim_AttackStart()
    {
        OnAttackStart?.Invoke();
    }

    public void Anim_AttackHit()
    {
        OnAttackHit?.Invoke();
    }

    public void Anim_AttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    public void Anim_HitStart()
    {
        OnHitStart?.Invoke();
    }

    public void Anim_Hit()
    {
        OnHit?.Invoke();
    }

    public void Anim_HitEnd()
    {
        OnHitEnd?.Invoke();
    }
}