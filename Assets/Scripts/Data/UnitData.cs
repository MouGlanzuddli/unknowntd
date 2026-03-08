using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite icon;

    [Header("Faction")]
    public Faction faction;

    public int maxHealth;
    public int damage;

    public float attackRange;
    public float aggroRange = 5f;

    public float moveSpeed;
    public bool isFacingRight = true;

    public GameObject prefab;

    [Header("Shop")]
    public int costGold;
    public bool purchasable;

    [Header("Level")]
    public UnitLevelData levelData;
}