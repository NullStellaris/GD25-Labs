using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class OverlayManager : Singleton<OverlayManager> {
    public Canvas gameOverScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;

    void Start() {
        gameOverScreen.enabled = false;
    }

    public void DrawScore(int score) {
        scoreText.text = "Score: " + score.ToString();
    }

    public void GameOver(int score) {
        gameOverScreen.enabled = true;
        gameOverText.text = "Game Over!<br><br>Score: " + score.ToString();
    }

    public void OnReset() {
        gameOverScreen.enabled = false;
        DrawScore(0);
    }
}
