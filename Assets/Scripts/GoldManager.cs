using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : BaseSingleton<GoldManager>
{
    [Header("Gold Group Root")]
    [SerializeField] private Transform goldGroup;

    [Header("Runtime Gold List")]
    [SerializeField] private List<Transform> goldNodes = new();

    private Dictionary<Transform, List<PawnController>> goldWorkers = new();

    public IReadOnlyList<Transform> GoldNodes => goldNodes;

    protected override void Awake()
    {
        base.Awake();
        CollectGoldNodes();
    }

    public void CollectGoldNodes()
    {
        goldNodes.Clear();
        goldWorkers.Clear();

        if (goldGroup == null)
        {
            Debug.LogWarning("GoldGroup not assigned!");
            return;
        }

        Transform[] all = goldGroup.GetComponentsInChildren<Transform>(true);

        foreach (var t in all)
        {
            if (t.CompareTag("Gold"))
            {
                goldNodes.Add(t);
                goldWorkers[t] = new List<PawnController>();
            }
        }
    }

    public Transform GetBestGold(PawnController pawn)
    {
        Transform best = null;
        int minWorkers = int.MaxValue;

        foreach (var gold in goldNodes)
        {
            if (gold == null)
                continue;

            int workerCount = goldWorkers[gold].Count;

            if (workerCount < minWorkers)
            {
                minWorkers = workerCount;
                best = gold;
            }
        }

        if (best != null)
        {
            RegisterPawn(best, pawn);
        }

        return best;
    }

    public void RegisterPawn(Transform gold, PawnController pawn)
    {
        if (gold == null || pawn == null)
            return;

        if (!goldWorkers.ContainsKey(gold))
            goldWorkers[gold] = new List<PawnController>();

        if (!goldWorkers[gold].Contains(pawn))
        {
            goldWorkers[gold].Add(pawn);
        }
    }

    public void UnregisterPawn(Transform gold, PawnController pawn)
    {
        if (gold == null || pawn == null)
            return;

        if (!goldWorkers.ContainsKey(gold))
            return;

        goldWorkers[gold].Remove(pawn);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (goldGroup != null)
            CollectGoldNodes();
    }
#endif
}