using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayLocalVoiceLine : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip voiceLine;

    public void Play()
    {
        AudioSource.PlayClipAtPoint(voiceLine, transform.position);
    }
}
