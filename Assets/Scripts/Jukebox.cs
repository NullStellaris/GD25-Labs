using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Track {
    public string name;
    public AudioClip clip;
    public bool loop;
    public AudioMixerGroup output;

    [HideInInspector]
    public AudioSource source;

    public void Play() {
        source.Play();
    }

    public void Stop() {
        source.Stop();
    }

    public void Pause() {
        source.Pause();
    }

    public void UnPause() {
        source.UnPause();
    }
}

public class Jukebox : Singleton<Jukebox> {
    public Track[] tracks;

    void Start() {
        foreach (Track t in tracks) {
            t.source = gameObject.AddComponent<AudioSource>();
            t.source.clip = t.clip;
            t.source.loop = t.loop;
            t.source.outputAudioMixerGroup = t.output;
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

    public void PauseAll() {
        foreach (Track track in tracks) {
            track.Pause();
        }
    }

    public void ResumeAll() {
        foreach (Track track in tracks) {
            track.UnPause();
        }
    }
}

