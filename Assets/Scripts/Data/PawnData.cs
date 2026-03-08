using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pawn", menuName = "Game/Units/Pawn")]
public class PawnData : UnitData
{
    [Header("Pawn Data")]
    public float mineCooldown;
}