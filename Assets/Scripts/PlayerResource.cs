using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerResource : BaseSingleton<PlayerResource>
{
    [SerializeField] private int startingGold = 100;
    private int currentGold;

    private TextMeshProUGUI goldText;

    public int Gold => currentGold;

    protected override void Awake()
    {
        base.Awake();
        currentGold = startingGold;
    }

    public void Init(TextMeshProUGUI text)
    {
        goldText = text;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        currentGold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
            return false;

        currentGold -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (goldText != null)
        {
            goldText.text = currentGold.ToString();
        }
    }
}