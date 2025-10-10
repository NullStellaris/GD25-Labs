using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class OverlayManager : Singleton<OverlayManager> {
    public Canvas gameOverScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;

    // SOs
    public IntVariable score;
    public GameEvent GlobalReset;
    public GameEvent RefreshScore;
    public GameEvent GameOver;

    // Register Event SO listeners
    void Start() {
        GlobalReset.RegisterListener(OnReset);
        RefreshScore.RegisterListener(OnDrawScore);
        GameOver.RegisterListener(OnGameOver);

        gameOverScreen.enabled = false;
    }

    public void OnDrawScore() {
        scoreText.text = "Score: " + score.Value.ToString();
    }

    public void OnGameOver() {
        gameOverScreen.enabled = true;
        gameOverText.text = "Game Over!<br><br>Score: " + score.Value.ToString();
    }

    public void OnReset() {
        gameOverScreen.enabled = false;
        OnDrawScore();
    }
}
