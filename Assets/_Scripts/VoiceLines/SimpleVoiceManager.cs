using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleVoiceManager : Singleton<SimpleVoiceManager>
{
    private AudioSource source;
    [SerializeField] private SubtitleChannel subtitleChannel;

    //VoicelineData voiceline;
    private readonly Queue<VoicelineData> queue = new();
    float clipLength;

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
    }


    public void QueueVoiceLine(VoicelineData voiceline)
    {
        if (source.isPlaying)
        {
            queue.Enqueue(voiceline);
        }
        else
        {
            PlayVoiceLine(voiceline);
        }
    }

    public void PlayVoiceLine(VoicelineData voiceline)
    {
        if(voiceline)
            PlayVoiceLine(voiceline.audioClip, voiceline.subtitle, voiceline.time, voiceline.color);
    }
    public void PlayVoiceLine(AudioClip clip, string subtitleText, float subtitleTime, Color subtitleColor)
    {
        //this.voiceline = voiceline;
        source.clip = clip;
        source.Play();

        clipLength = clip.length;

        if (subtitleChannel)
        {
            SubtitleData subtitle = new()
            {
                text = subtitleText,
                time = subtitleTime,
                color = subtitleColor
            };
            subtitleChannel.Raise(subtitle);
        }

    }


    public void Update()
    {
        if (source.isPlaying) {
            float t = source.time / clipLength;

        }

        if(queue.TryDequeue(out VoicelineData voiceline))
        {
            PlayVoiceLine(voiceline);
        }
    }
}
