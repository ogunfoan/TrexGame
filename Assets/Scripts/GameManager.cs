using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI & Flow")]
    public GameObject gameOverPanel;
    public ScoreManager scoreManager;
    
    void Start()
    {
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        // Oyun durduğunda (Time.timeScale=0) kaydırma yapma
        if (Time.timeScale == 0f) return;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        if (scoreManager != null) scoreManager.CheckHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // İsteğe bağlı: dışarıdan zorluk arttıkça parallax hızını yükseltmek için
}
