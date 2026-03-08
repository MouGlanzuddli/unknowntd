using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : BaseSingleton<UnitManager>
{

    [SerializeField] private List<UnitData> units = new();

    private Dictionary<string, UnitData> unitLookup;
    public IReadOnlyList<UnitData> Units => units;
    protected override void Awake()
    {
        base.Awake();
        unitLookup = new Dictionary<string, UnitData>();

        foreach (var unit in units)
        {
            if (unit == null) continue;
            unitLookup[unit.unitName] = unit;
        }
    }

    public UnitData GetUnit(string unitName)
    {
        unitLookup.TryGetValue(unitName, out var unit);
        return unit;
    }
}