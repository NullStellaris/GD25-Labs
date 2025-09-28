using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Track {
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;

    public void Play() {
        source.Play();
    }

    public void Stop() {
        source.Stop();
    }
}

public class Jukebox : MonoBehaviour {
    public static Jukebox Instance;
    public Track[] tracks;

    void Start() {
        foreach (Track t in tracks) {
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.clip;
            t.source.volume = t.volume;
            t.source.loop = t.loop;
        }
    }
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }
    public void PlayOver(string name) {
        StopAll();
        Track track = Array.Find(tracks, track => track.name == name);
        if (track != null) {
            track.Play();
        }
        else {
            Debug.Log(name + " track does not exist!");
        }
    }

    public void PlaySimul(string name) {
        Track track = Array.Find(tracks, track => track.name == name);
        if (track != null) {
            track.Play();
        }
        else {
            Debug.Log(name + " track does not exist!");
        }
    }

    public void StopAll() {
        foreach (Track track in tracks) {
            track.Stop();
        }
    }
}
