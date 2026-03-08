using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private int slotCount = 8;

    private List<Transform> workers = new();
    private Vector3[] slots;

    private void Awake()
    {
        GenerateSlots();
    }

    private void GenerateSlots()
    {
        slots = new Vector3[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            float angle = i * Mathf.PI * 2f / slotCount;

            Vector3 offset = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0
            ) * radius;

            slots[i] = transform.position + offset;
        }
    }

    public Vector3 GetFreeSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool occupied = false;

            foreach (var worker in workers)
            {
                if (Vector3.Distance(worker.position, slots[i]) < 0.2f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                return slots[i];
        }

        return slots[0];
    }

    public void RegisterWorker(Transform pawn)
    {
        if (!workers.Contains(pawn))
            workers.Add(pawn);
    }

    public void RemoveWorker(Transform pawn)
    {
        workers.Remove(pawn);
    }
}
