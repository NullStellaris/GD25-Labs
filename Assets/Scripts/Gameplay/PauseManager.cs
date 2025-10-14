using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public PlayerMovement player;
    public AudioSource bgmSource;

    private bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Debug.Log("Pause");
        Time.timeScale = 0f;
        player.canMove = false;
        pauseMenu.SetActive(true);
        bgmSource.Pause();
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("Resume");
        Time.timeScale = 1f;
        player.canMove = true;
        pauseMenu.SetActive(false);
        bgmSource.UnPause();
        isPaused = false;
    }
}
