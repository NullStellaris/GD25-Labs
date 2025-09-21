using System.Linq;
using UnityEngine;

public class Jukebox : MonoBehaviour {
    public GameObject audioLibrary;

    private AudioSource[] GetAllSatellites() {
        return audioLibrary.GetComponentsInChildren<AudioSource>();
    }

    public void PlayOver(string name, bool loop) {
        StopAll();
        Transform queryResult = audioLibrary.transform.Find(name);
        AudioSource satellite = queryResult.gameObject.GetComponent<AudioSource>();
        if (queryResult != null) {
            satellite.loop = loop;
            satellite.Play();
        }
        else {
            Debug.Log(name + " track does not exist!");
        }
    }

    public void PlaySimul(string name, bool loop) {
        Transform queryResult = audioLibrary.transform.Find(name);
        AudioSource satellite = queryResult.gameObject.GetComponent<AudioSource>();
        if (queryResult != null) {
            satellite.loop = loop;
            satellite.Play();
        }
        else {
            Debug.Log(name + " track does not exist!");
        }
    }

    public void StopAll() {
        foreach (AudioSource satellite in GetAllSatellites()) {
            satellite.Stop();
        }
    }

    public bool IsPlaying() {
        return GetAllSatellites().Any(b => b == true);
    }
}
