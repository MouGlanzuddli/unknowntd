using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    private bool isGameOver = false;

    public int CurrentLevelIndex { get; set; } = 1;

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        // Unlock next level
        UnlockLevel(CurrentLevelIndex + 1);

        Time.timeScale = 0f;
        GameUI.Instance.ShowWinPanel();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayWin();
    }

    public void LoseGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f;
        GameUI.Instance.ShowLosePanel();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLose();
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SelectMap");
    }

    public void LoadMap(int index)
    {
        CurrentLevelIndex = index;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map_" + index);
    }

    public void UnlockLevel(int index)
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (index > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", index);
            PlayerPrefs.Save();
        }
    }

    public bool IsLevelUnlocked(int index)
    {
        if (index <= 1) return true;
        return PlayerPrefs.GetInt("UnlockedLevel", 1) >= index;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
