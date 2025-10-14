using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    public IntVariable gameScore;
    private int initalScore;
    public AudioSource bgAudio;
    void Start()
    {
        Debug.Log("start");
        initalScore = 0;
        gameScore.Value = 0;
        gameStart.Invoke();
        // subscribe to scene manager scene change
        Time.timeScale = 1.0f;
        SceneManager.activeSceneChanged += SceneSetup;
        bgAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        Debug.Log("GameRestart");
        // reset score
        gameScore.Value = initalScore;
        SetScore(initalScore);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
        bgAudio.Stop();
        bgAudio.Play();
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Debug.Log("GameOver");
        Time.timeScale = 0.0f;
        bgAudio.Stop();
        gameOver.Invoke();
    }

    public void SceneSetup(Scene current, Scene next)
    {
        bgAudio.Stop();
        if (next.name == "World 1-1")
        {
            bgAudio.Play();
            gameScore.Value = 0;
        }
        if (next.name == "World 1-2")
        {
            bgAudio.Play();
        }
        initalScore = gameScore.Value;
        gameStart.Invoke();
        SetScore(gameScore.Value);
    }
}