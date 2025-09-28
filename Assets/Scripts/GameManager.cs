using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GameManager : MonoBehaviour {
    public PlayerMovement player;
    public GameObject enemies;
    public GameObject obstacles;
    public Canvas gameOverScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;

    [System.NonSerialized] public int score = 0; // we don't want this to show up in the inspector
    [System.NonSerialized] public int countScoreState = -1;

    public static GameManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        Jukebox.Instance.PlayOver("level");
        gameOverScreen.enabled = false;
    }

    void OnEnable() {
        player.onDamaged.AddListener(HandlePlayerDeath);
        player.onScore.AddListener(HandleScore);
    }

    void OnDisable() {
        player.onDamaged.RemoveListener(HandlePlayerDeath);
        player.onScore.RemoveListener(HandleScore);
    }

    void SetButtonsInteractable(bool state) {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons) {
            button.interactable = state;
        }
    }

    public void DrawScore() {
        scoreText.text = "Score: " + score.ToString();
    }

    void HandleScore(int gain) {
        score += gain;
        DrawScore();
    }

    void HandlePlayerDeath() {
        StartCoroutine(GameOver());
    }


    IEnumerator GameOver() {
        Time.timeScale = 0.0f;
        SetButtonsInteractable(false);
        Jukebox.Instance.PlayOver("dead");
        yield return new WaitForSecondsRealtime(3.5f);
        gameOverScreen.enabled = true;
        SetButtonsInteractable(true);
        gameOverText.text = "Game Over!<br><br>Score: " + score.ToString();
    }

    public void RestartButtonCallback(int input) {
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    public static event Action GlobalReset;
    private void ResetGame() {
        // clear gameOver screen
        gameOverScreen.enabled = false;
        // reset score
        score = 0;
        DrawScore();
        // reset level audio
        Jukebox.Instance.PlayOver("level");
        GlobalReset?.Invoke();
    }

    void FixedUpdate() {
        // mario jumps
        if (player.jumpState) {
            countScoreState = 0;
        }

        // when jumping, and Goomba is near Mario and we haven't registered our score
        if (!player.onGroundState && countScoreState == 0) {
            Transform[] enemyLocations = enemies.GetComponentsInChildren<Transform>();
            foreach (Transform enemyLocation in enemyLocations) {
                if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f && enemyLocation.gameObject.CompareTag("Enemy") && enemyLocation.position.y < transform.position.y) {
                    countScoreState = 1;
                }
            }
        }
        else if (player.onGroundState) {
            if (countScoreState == 1) {
                score++;
                DrawScore();
            }
            countScoreState = -1;
        }
    }
}
