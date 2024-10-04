using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayVoiceline : MonoBehaviour
{
    [SerializeField] private VoicelineData voiceline;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private float delay = 1;

    public void Play()
    {
        SimpleVoiceManager.Instance.QueueVoiceLine(voiceline);
    }

    void Start()
    {
        if (playOnStart)
        {
            PlayDelayed();
        }
    }

    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Play();
    }

    public void PlayDelayed()
    {
        StartCoroutine(PlayAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
