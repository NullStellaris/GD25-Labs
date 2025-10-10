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
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : Singleton<GameManager> {
    // Utility Assets
    public AudioMixer mixer;
    [NonSerialized] public int countScoreState = -1;

    // SO Events
    // send
    public GameEvent GlobalReset;
    public GameEvent RefreshScore;
    public GameEvent GameOver;
    // recv
    public GameEvent TookDamage;
    public IntGameEvent ScoreGain;

    // SOs
    public IntVariable score;
    public StringVariable lastScene;

    void Start() {
        // Register SO Listeners
        TookDamage.RegisterListener(OnPlayerDeath);
        ScoreGain.RegisterListener(OnScore);
        Jukebox.Instance.PlayOver("level");

        // Reset SO values
        score.Value = 0;
    }

    void SetButtonsInteractable(bool state) {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons) {
            button.interactable = state;
        }
    }

    public void OnScore(int gain) {
        score.Add(gain);
        RefreshScore.Invoke();
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
        GameOver.Invoke();
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
        score.Value = 0;
        // reset level audio
        Jukebox.Instance.PlayOver("level");
    }
}
