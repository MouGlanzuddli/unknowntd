using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseSingleton<GameManager>
{
    private bool isGameOver = false;

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        
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
        SceneManager.LoadScene("GamePlay");
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
