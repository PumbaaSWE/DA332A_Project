using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private float stepLength = 1;
    [SerializeField] private float stepLengthRunning = 1.4f;
    [SerializeField] private float stepVelocity = 1;
    [SerializeField] private float runVelocity = 5;
    [SerializeField][Range(0, 1)] private float volume = 1;
    [SerializeField] private AudioMixerGroup mixerGroup;


    AudioSource source0;
    AudioSource source1;
    Vector3 lastPos;
    Vector3 stepPos;
    bool left;

    void Start()
    {

        InitSource(ref source0);
        InitSource(ref source1);

        lastPos = transform.position;

    }

    public void InitSource(ref AudioSource source)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = mixerGroup;
        source.playOnAwake = false;
        source.volume = volume;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 delta = (lastPos - pos).WithY() ;

        float speed = delta.magnitude / Time.deltaTime;
        float len = speed > runVelocity ? stepLengthRunning : stepLength; //is sprinting from move controllern

        if (speed > stepVelocity  && Vector3.Distance(stepPos, pos) > len)
        {
            stepPos = pos;
            left = !left;
            TakeStep(left ? source0 : source1, sounds);
        }

        lastPos = pos;
    }

    public void TakeStep(AudioSource source, AudioClip[] sounds)
    {
        source.clip = sounds[Random.Range(0, sounds.Length)];
        source.pitch = Random.Range(.9f, 1.1f);
        source.Play();          
    }
}
