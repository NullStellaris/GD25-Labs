using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour {
    // Utility Assets
    public AudioMixer mixer;

    [NonSerialized] public int score = 0; // we don't want this to show up in the inspector
    [NonSerialized] public int countScoreState = -1;

    public static GameManager Instance;

    // Events
    public UnityEvent GlobalReset;
    public UnityEvent<int> RefreshScore;
    public UnityEvent<int> GameOver;


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
    }

    void SetButtonsInteractable(bool state) {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons) {
            button.interactable = state;
        }
    }

    public void OnScore(int gain) {
        score += gain;
        RefreshScore.Invoke(score);
    }

    public void OnPlayerDeath() {
        StartCoroutine(PlayerDeath());
    }


    IEnumerator PlayerDeath() {
        Time.timeScale = 0.0f;
        SetButtonsInteractable(false);
        Jukebox.Instance.PlayOver("dead");
        yield return new WaitForSecondsRealtime(3.5f);
        SetButtonsInteractable(true);
        GameOver.Invoke(score);
    }

    public void RestartButtonCallback(int input) {
        // reset everything
        GlobalReset.Invoke();
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame() {
        // reset score
        score = 0;
        // reset level audio
        Jukebox.Instance.PlayOver("level");
    }
}
