using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : BaseSingleton<GameUI>
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Win Panel UI")]
    [SerializeField] private TextMeshProUGUI winLevelText;
    [SerializeField] private TextMeshProUGUI winWaveText;
    [SerializeField] private Button winRetryButton;
    [SerializeField] private Button winMenuButton;

    [Header("Lose Panel UI")]
    [SerializeField] private TextMeshProUGUI loseLevelText;
    [SerializeField] private TextMeshProUGUI loseWaveText;
    [SerializeField] private Button loseRetryButton;
    [SerializeField] private Button loseMenuButton;

    [Header("Start Menu UI")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (PlayerResource.Instance != null)
            PlayerResource.Instance.Init(goldText);
            
        UpdateLevelUI();
        
        LevelManager.OnWaveChanged += UpdateLevelUI;

        if (winRetryButton != null) winRetryButton.onClick.AddListener(() => { GameManager.Instance.Retry(); PlayClickSound(); });
        if (winMenuButton != null) winMenuButton.onClick.AddListener(() => { GameManager.Instance.BackToMenu(); PlayClickSound(); });
        if (loseRetryButton != null) loseRetryButton.onClick.AddListener(() => { GameManager.Instance.Retry(); PlayClickSound(); });
        if (loseMenuButton != null) loseMenuButton.onClick.AddListener(() => { GameManager.Instance.BackToMenu(); PlayClickSound(); });

        if (playButton != null) playButton.onClick.AddListener(() => { GameManager.Instance.StartGame(); PlayClickSound(); });
        if (quitButton != null) quitButton.onClick.AddListener(() => { GameManager.Instance.QuitGame(); PlayClickSound(); });

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        
        if (countdownText != null) 
            countdownText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LevelManager.OnWaveChanged -= UpdateLevelUI;
    }

    private void UpdateLevelUI()
    {
        if (LevelManager.Instance == null) return;

        if (levelNameText != null)
            levelNameText.text = "Level: " + LevelManager.Instance.CurrentLevelName;

        if (waveText != null)
            waveText.text = $"Wave: {LevelManager.Instance.CurrentWaveNumber}/{LevelManager.Instance.TotalWavesInLevel}";
    }

    public void UpdateCountdown(float time)
    {
        if (countdownText == null) return;

        if (time <= 0)
        {
            if (countdownText.gameObject.activeSelf)
                countdownText.gameObject.SetActive(false);
            return;
        }

        if (!countdownText.gameObject.activeSelf)
            countdownText.gameObject.SetActive(true);

        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 100);

        countdownText.text = $"Next Wave In: {seconds:00}:{milliseconds:00}";
    }

    public void ShowWinPanel()
    {
        if (LevelManager.Instance == null) return;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winLevelText != null) winLevelText.text = $"Level: {LevelManager.Instance.CurrentLevelName}";
            if (winWaveText != null) 
                winWaveText.text = $"Waves Completed: {LevelManager.Instance.TotalWavesCompleted}/{LevelManager.Instance.TotalWavesAcrossAllLevels}";
        }
    }

    public void ShowLosePanel()
    {
        if (LevelManager.Instance == null) return;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
            if (loseLevelText != null) loseLevelText.text = $"Level: {LevelManager.Instance.CurrentLevelName}";
            if (loseWaveText != null) 
                loseWaveText.text = $"Waves Completed: {LevelManager.Instance.TotalWavesCompleted}/{LevelManager.Instance.TotalWavesAcrossAllLevels}";
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
    }
}