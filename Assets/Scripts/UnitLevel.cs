using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitLevel
{
    private UnitLevelData levelData;
    private int currentLevelIndex = 0;
    private int currentExp = 0;
    private Image expBar;
    private TextMeshProUGUI levelText;

    public event Action<int> OnLevelUp;
    public event Action<int, int> OnExpChanged; // currentExp, requiredExp

    public int CurrentLevel => currentLevelIndex + 1;
    public int CurrentExp => currentExp;
    
    public int RequiredExp 
    {
        get
        {
            if (levelData == null || currentLevelIndex >= levelData.levels.Length - 1) return -1;
            return levelData.levels[currentLevelIndex].requiredExp;
        }
    }

    public UnitLevel(UnitLevelData levelData, Image expBar = null, TextMeshProUGUI levelText = null)
    {
        this.levelData = levelData;
        this.expBar = expBar;
        this.levelText = levelText;
        currentLevelIndex = 0;
        currentExp = 0;
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        if (levelData == null) return;
        if (currentLevelIndex >= levelData.levels.Length - 1) 
        {
            currentExp = 0;
            UpdateUI();
            return;
        }

        currentExp += amount;

        while (currentLevelIndex < levelData.levels.Length - 1 && currentExp >= levelData.levels[currentLevelIndex].requiredExp)
        {
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp, RequiredExp);
        UpdateUI();
    }

    private void LevelUp()
    {
        int required = levelData.levels[currentLevelIndex].requiredExp;
        currentExp -= required;
        currentLevelIndex++;
        
        OnLevelUp?.Invoke(CurrentLevel);
        UpdateUI();

        if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgrade();
    }

    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = "LV: " + CurrentLevel.ToString();
        }

        if (expBar == null || levelData == null) return;

        int required = RequiredExp;
        if (required <= 0)
        {
            expBar.fillAmount = 1;
            return;
        }

        expBar.fillAmount = (float)currentExp / required;
    }

    public UnitLevelData.LevelInfo GetCurrentLevelInfo()
    {
        int index = Mathf.Clamp(currentLevelIndex, 0, levelData.levels.Length - 1);
        return levelData.levels[index];
    }
}
