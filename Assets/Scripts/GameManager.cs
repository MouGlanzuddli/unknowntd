using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private int startingGold = 100;
    [SerializeField] private int startingLives = 20;

    [Header("Events")]
    public UnityEvent<int> OnGoldChanged;
    public UnityEvent<int> OnLivesChanged;
    public UnityEvent<int> OnWaveStarted;
    public UnityEvent<int> OnWaveCompleted;
    public UnityEvent OnGameOver;
    public UnityEvent OnVictory;

    private int currentGold;
    private int currentLives;
    private bool isGameOver = false;

    public int Gold => currentGold;
    public int Lives => currentLives;
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        currentGold = startingGold;
        currentLives = startingLives;
    }

    private void Start()
    {
        OnGoldChanged?.Invoke(currentGold);
        OnLivesChanged?.Invoke(currentLives);
    }

    // ======================
    // GOLD MANAGEMENT
    // ======================
    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            return true;
        }
        return false;
    }

    // ======================
    // LIVES MANAGEMENT
    // ======================
    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        currentLives -= damage;
        currentLives = Mathf.Max(currentLives, 0);
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    // ======================
    // WAVE MANAGEMENT
    // ======================
    public void OnWaveStart(int waveIndex)
    {
        Debug.Log($"Wave {waveIndex + 1} started!");
        OnWaveStarted?.Invoke(waveIndex);
    }

    public void OnWaveComplete(int waveIndex)
    {
        Debug.Log($"Wave {waveIndex + 1} completed!");
        OnWaveCompleted?.Invoke(waveIndex);
    }

    public void OnAllWavesCompleted()
    {
        Debug.Log("Victory! All waves defeated!");
        OnVictory?.Invoke();
    }

    // ======================
    // GAME STATE
    // ======================
    private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");
        OnGameOver?.Invoke();

        // Stop time or show game over screen
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
