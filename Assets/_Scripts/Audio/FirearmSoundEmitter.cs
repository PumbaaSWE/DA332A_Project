using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirearmSoundEmitter : MonoBehaviour
{
    [SerializeField] AudioSource SoundEmitter;
    [SerializeField] List<SoundStage> Stages;

    public void MakeSound(int soundToPlay)
    {
        SoundEmitter.clip = Stages[soundToPlay].GetClip();
        SoundEmitter.pitch = Stages[soundToPlay].GetPitch();
        SoundEmitter.Play();
    }
}

[Serializable]
public class SoundStage
{
    public List<AudioClip> Clips;
    public float Pitch = 1;
    public float PitchVariance = 0;

    public AudioClip GetClip()
    {
        return Clips[Random.Range(0, Clips.Count)];
    }

    public float GetPitch()
    {
        return Pitch + Random.Range(-PitchVariance, PitchVariance);
    }
}