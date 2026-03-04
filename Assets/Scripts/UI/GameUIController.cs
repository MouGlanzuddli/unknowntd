using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý UI thắng/thua của game.
/// - Kết nối vào GameManager.OnGameOver và GameManager.OnVictory qua Inspector.
/// - Các panel Panel_GameOver và Panel_Victory phải được assign trong Inspector.
/// </summary>
public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelVictory;

    [Header("GameOver UI")]
    [SerializeField] private TextMeshProUGUI txtGameOverTitle;
    [SerializeField] private TextMeshProUGUI txtGameOverSubtitle;
    [SerializeField] private Button btnRestartGameOver;

    [Header("Victory UI")]
    [SerializeField] private TextMeshProUGUI txtVictoryTitle;
    [SerializeField] private TextMeshProUGUI txtVictorySubtitle;
    [SerializeField] private Button btnRestartVictory;

    [Header("HUD (optional)")]
    [SerializeField] private GameObject hudPanel;         // Panel HUD chính (thanh lives, gold...)
    [SerializeField] private TextMeshProUGUI txtLives;
    [SerializeField] private TextMeshProUGUI txtGold;
    [SerializeField] private TextMeshProUGUI txtWave;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private CanvasGroup victoryCanvasGroup;

    // Stats để hiển thị cuối game
    private int enemiesKilled = 0;

    private void Awake()
    {
        // Ẩn cả 2 panel lúc đầu
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictory != null) panelVictory.SetActive(false);

        // Gán nút
        if (btnRestartGameOver != null)
            btnRestartGameOver.onClick.AddListener(RestartGame);
        if (btnRestartVictory != null)
            btnRestartVictory.onClick.AddListener(RestartGame);
    }

    // ======================
    // HUD UPDATES
    // (Kết nối vào GameManager Events qua Inspector)
    // ======================

    /// <summary>Gọi từ GameManager.OnLivesChanged event.</summary>
    public void UpdateLives(int lives)
    {
        if (txtLives != null)
            txtLives.text = $"❤️ {lives}";
    }

    /// <summary>Gọi từ GameManager.OnGoldChanged event.</summary>
    public void UpdateGold(int gold)
    {
        if (txtGold != null)
            txtGold.text = $"🪙 {gold}";
    }

    /// <summary>Gọi từ GameManager.OnWaveStarted event.</summary>
    public void UpdateWave(int waveIndex)
    {
        if (txtWave != null)
            txtWave.text = $"Wave {waveIndex + 1}";
    }

    /// <summary>Gọi mỗi khi 1 kẻ địch chết (optional, để hiện thống kê).</summary>
    public void OnEnemyKilled()
    {
        enemiesKilled++;
    }

    // ======================
    // GAME OVER
    // ======================

    /// <summary>
    /// Gọi từ GameManager.OnGameOver event trong Inspector.
    /// </summary>
    public void ShowGameOver()
    {
        StartCoroutine(ShowPanelWithFade(panelGameOver, gameOverCanvasGroup,
            txtGameOverSubtitle, $"Kẻ địch đã phá vỡ phòng thủ!\nTiêu diệt được: {enemiesKilled} kẻ địch"));
    }

    // ======================
    // VICTORY
    // ======================

    /// <summary>
    /// Gọi từ GameManager.OnVictory event trong Inspector.
    /// </summary>
    public void ShowVictory()
    {
        StartCoroutine(ShowPanelWithFade(panelVictory, victoryCanvasGroup,
            txtVictorySubtitle, $"Tất cả wave đã bị tiêu diệt!\nTổng kẻ địch tiêu diệt: {enemiesKilled}"));
    }

    // ======================
    // ANIMATION + HELPERS
    // ======================
    private IEnumerator ShowPanelWithFade(GameObject panel, CanvasGroup canvasGroup, TextMeshProUGUI subtitle, string subtitleText)
    {
        if (panel == null) yield break;

        // Cập nhật text thống kê
        if (subtitle != null)
            subtitle.text = subtitleText;

        panel.SetActive(true);

        // Fade in
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // dùng unscaled vì Time.timeScale = 0
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
