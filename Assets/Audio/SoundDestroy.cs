using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    private AudioSource audioSource;
    private float clipLength;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        clipLength = audioSource.clip.length;
        yield return new WaitForSeconds(clipLength);
        // TODO release audio back into pool
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
