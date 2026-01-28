using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxUnits = 5;

    private float timer;
    private int currentUnits;

    private void Update()
    {
        if (unitPrefab == null || spawnPoint == null)
            return;

        if (currentUnits >= maxUnits)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnUnit();
            timer = 0f;
        }
    }

    private void SpawnUnit()
    {
        GameObject unit = Instantiate(
            unitPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        currentUnits++;


    }

    public void NotifyUnitDestroyed()
    {
        currentUnits--;
        if (currentUnits < 0)
            currentUnits = 0;
    }
}
