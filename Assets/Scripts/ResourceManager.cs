using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : BaseSingleton<ResourceManager>
{
    private int gold;
    private TextMeshProUGUI goldText;

    public void Init(TextMeshProUGUI goldTextUI = null)
    {
        goldText = goldTextUI;
        UpdateUI();
    }

    public int GetGold()
    {
        return gold;
    }

    public void SetGold(int amount)
    {
        gold = amount;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;

        gold -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }
}