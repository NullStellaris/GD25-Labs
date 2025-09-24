using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float remainingTime; //in seconds
    // Update is called once per frame
    void Update()
    {
        if(remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if(remainingTime < 0)
        {
            remainingTime = 0;
        }
        int min = Mathf.FloorToInt(remainingTime / 60);
        int sec = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}
