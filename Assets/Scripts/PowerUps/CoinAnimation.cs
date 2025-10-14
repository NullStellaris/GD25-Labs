using UnityEngine;
using System.Collections;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpDuration = 1f;

    private Vector3 startPos;
    private bool isAnimating = false;
    public AudioSource coinAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position; 
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    public void Jump()
    {
        //play sound
        //animate coin
        //add to score
        //disable coin?
        if (!isAnimating)
            StartCoroutine(JumpRoutine());
    }
    private IEnumerator JumpRoutine()
    {
        coinAudio.PlayOneShot(coinAudio.clip);
        isAnimating = true;
        float halfDuration = jumpDuration / 2f;
        float timer = 0f;//track the elapsed
        Vector3 peakPos = startPos + Vector3.up * jumpHeight;// target position for Lerp

        // Move up
        while (timer < halfDuration)
        {
            transform.position = Vector3.Lerp(startPos, peakPos, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;// wait for next frame
        }
        transform.position = peakPos;

        // Move down
        timer = 0f;
        while (timer < halfDuration)
        {
            transform.position = Vector3.Lerp(peakPos, startPos, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        isAnimating = false;
    }
}
