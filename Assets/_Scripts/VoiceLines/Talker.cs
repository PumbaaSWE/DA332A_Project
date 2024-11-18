using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Talker : MonoBehaviour
{

    [SerializeField] private VoicelineData[] voicelines;
    [SerializeField] private SubtitleChannel subtitleChannel;
    private AudioSource source;

    private readonly Queue<VoicelineData> queue = new();
    float clipLength;

    void Start()
    {
        source = GetComponent<AudioSource>();
        foreach (var vl in voicelines)
        {
            queue.Enqueue(vl);
        }
        enabled = false;
    }

    public void Update()
    {
        if (source.isPlaying || clipLength > 0)
        {
            clipLength -= Time.deltaTime;
            //float t = source.time / clipLength;
            return;
        }

        if (queue.TryDequeue(out VoicelineData voiceline))
        {
            PlayVoiceLine(voiceline);
        }
    }

    public void Play()
    {
        enabled = true;
        //if(voicelines == null || voicelines.Length <= 0) return;
        //PlayVoiceLine(voicelines[0]);
    }

    public void PlayVoiceLine(VoicelineData voiceline)
    {
        if (voiceline)
            PlayVoiceLine(voiceline.audioClip, voiceline.subtitle, voiceline.time, voiceline.color);
    }
    public void PlayVoiceLine(AudioClip clip, string subtitleText, float overrideTime, Color subtitleColor)
    {
        //TODO check if player is to far from sourche and dont play or do subtitles -> disable and reset trigger? = play where it ended...


        //this.voiceline = voiceline;
        source.clip = clip;
        source.Play();

        //if(source.maxDistance)
        if (subtitleChannel)
        {
            SubtitleData subtitle = new()
            {
                text = subtitleText,
                time = overrideTime,
                color = subtitleColor
            };
            subtitleChannel.Raise(subtitle);
        }

    }
}
