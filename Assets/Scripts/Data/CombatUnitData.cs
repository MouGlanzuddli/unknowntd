using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatUnit", menuName = "Game/Units/Combat")]
public class CombatUnitData : UnitData
{
    [Header("Combat Unit Data")]
    public float attackCooldown;
}
