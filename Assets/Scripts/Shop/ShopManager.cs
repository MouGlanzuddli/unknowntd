using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : BaseSingleton<ShopManager>
{
    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject shopItemPrefab;

    private void Start()
    {
        BuildShop();
    }

    private void BuildShop()
    {
        foreach (var unit in UnitManager.Instance.Units)
        {
            if (!unit.purchasable) continue;

            GameObject item = Instantiate(shopItemPrefab, content);

            Image icon = item.transform.Find("IconImage").GetComponent<Image>();
            TextMeshProUGUI price = item.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            Button button = item.GetComponent<Button>();

            icon.sprite = unit.icon;
            price.text = "x" + unit.costGold.ToString();

            button.onClick.AddListener(() => Purchase(unit));
        }
    }

    private void Purchase(UnitData unit)
    {
        if (PlayerResource.Instance.Gold < unit.costGold)
        {
            Debug.Log("Not enough gold");
            AudioManager.Instance.PlayCancel();
            return;
        }

        AudioManager.Instance.PlayClick();

        PlayerResource.Instance.SpendGold(unit.costGold);

        PlayerUnitManager.Instance.AddUnit(unit);
    }
}