using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public IntVariable gameScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadingScreen()
    {
        SceneManager.LoadSceneAsync("Loading scene", LoadSceneMode.Single);
        Debug.Log("Go to laoding scene");
    }
    public void resetScore()
    {
        gameScore.previousHighestValue = 0;
    }
}
