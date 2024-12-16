using UnityEngine;

/**
 * I know there is another one very similar, this one is not to be tied to the Singleton however
 * also respect the mixer settings, but it is not done yet
 * 
 * WIP
 * 
 * -Jack
 * 
 * */

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{

    AudioSource audioSource;    

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.Play();
    }
}
