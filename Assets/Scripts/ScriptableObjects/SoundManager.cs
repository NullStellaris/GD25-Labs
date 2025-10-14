using UnityEngine;

[CreateAssetMenu(fileName = "SoundManager", menuName = "ScriptableObjects/SoundManager")]
public class SoundManager : ScriptableObject
{
    // Use audioSource instead of game object 
    public AudioSource SoundObject;

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<SoundManager>("SoundManager");

                Debug.Log(instance == null ? "Not found!" : "Loaded OK!");
            }
            return instance;
        }
    }
    //Might want to add pitch in future
    public static void PlaysoundFXClip(AudioClip clip, Vector3 soundPos, float vol)
    {
        // TODO use audio pool instead of instantiate&destroy
        // Important to refrence the public Instance not the private instance
        AudioSource a = Instantiate(Instance.SoundObject, soundPos, Quaternion.identity);

        a.clip = clip;
        a.volume = vol;
        a.Play();
    }
}
