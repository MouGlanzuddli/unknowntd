using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    GameOver,
    Victory,
    Paused
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public GameState CurrentState { get; private set; }
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                gameOverPanel.SetActive(true);
                break;

            case GameState.Victory:
                Time.timeScale = 0f;
                victoryPanel.SetActive(true);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}