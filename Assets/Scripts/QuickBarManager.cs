using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickBarManager : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject itemPrefab;

    private Dictionary<UnitData, TextMeshProUGUI> itemLookup = new();

    private void Start()
    {
        BuildBar();

        PlayerUnitManager.Instance.OnUnitCountChanged += UpdateItem;
    }

    private void BuildBar()
    {
        foreach (var unit in UnitManager.Instance.Units)
        {
            if (!unit.purchasable) continue;

            GameObject item = Instantiate(itemPrefab, content);

            Image icon = item.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI count = item.transform.Find("Count").GetComponent<TextMeshProUGUI>();
            Button button = item.GetComponent<Button>();

            icon.sprite = unit.icon;

            int current = PlayerUnitManager.Instance.GetAvailableCount(unit);
            count.text = "x" + current.ToString();

            itemLookup[unit] = count;

            button.onClick.AddListener(() => SpawnUnit(unit));
        }
    }

    private void SpawnUnit(UnitData unit)
    {
        if (!PlayerUnitManager.Instance.CanSpawn(unit)) {
            AudioManager.Instance.PlayCancel();
            return;
        }

        AudioManager.Instance.PlayClick();

        Castle.Instance.SpawnUnit(unit);
    }

    private void UpdateItem(UnitData data, int count)
    {
        if (!itemLookup.ContainsKey(data)) return;

        itemLookup[data].text = count.ToString();
    }
}