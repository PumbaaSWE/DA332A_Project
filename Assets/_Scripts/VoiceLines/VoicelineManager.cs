using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoicelineManager : Singleton<VoicelineManager>
{
    private AudioSource source;
    [SerializeField] private SubtitleChannel subtitleChannel;

    private readonly Queue<VoicelineData> queue = new();
    float clipLength;
    float[] subtitleDuration;
    string[] subtitles;
    Color subtitleColor;
    int subtitleSequence;

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
    }


    public void QueueVoiceLine(VoicelineData voiceline)
    {
        if (source.isPlaying || queue.Count > 0)
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
        if (voiceline)
            PlayVoiceLine(voiceline.audioClip, voiceline.subtitles, voiceline.time, voiceline.subtitlesTimeStamps, voiceline.color);
    }

    public void PlayVoiceLine(AudioClip clip, string[] subtitleText, float voicelineTime, float[] subtitleTime, Color subtitleColor)
    {
        source.clip = clip;
        source.Play();

        clipLength = voicelineTime > 0.0f ? voicelineTime : clip.length;

        InitializeNewSubtitle(subtitleText, subtitleTime, subtitleColor);
    }

    void InitializeNewSubtitle(string[] subtitleText, float[] subtitleTime, Color subtitleColor)
    {
        subtitleSequence = 0;
        if (subtitleText == null || subtitleTime == null || subtitleText.Length == 0 || subtitleTime.Length != subtitleText.Length)
        {
            subtitles = new string[0];
            subtitleDuration = new float[0];
            return;
        }
        subtitleDuration = new float[subtitleTime.Length];
        subtitles = new string[subtitleText.Length];
        float timeTotal = 0.0f;
        for (int i = 0; i < subtitleText.Length; i++)
        {
            subtitleDuration[i] = subtitleTime[i] - timeTotal;
            subtitles[i] = subtitleText[i];
            timeTotal += subtitleDuration[i];
        }
        this.subtitleColor = subtitleColor;

        SetNewSubtitle(subtitleSequence);
    }

    void SetNewSubtitle(int i)
    {
        if (subtitleChannel)
        {
            SubtitleData subtitle = new()
            {
                text = subtitles[i],
                time = subtitleDuration[i],
                color = subtitleColor
            };
            subtitleChannel.Raise(subtitle);
        }
    }

    public void Update()
    {
        if(subtitles != null && subtitles.Length > subtitleSequence)
        {
            subtitleDuration[subtitleSequence] -= Time.deltaTime;

            if(subtitleDuration[subtitleSequence] <= 0)
            {
                subtitleSequence++;
                if(subtitles.Length > subtitleSequence)
                {
                    SetNewSubtitle(subtitleSequence);
                }
            }
        }

        if (source.isPlaying || clipLength > 0)
        {
            clipLength -= Time.deltaTime;
            return;
        }

        if (queue.TryDequeue(out VoicelineData voiceline))
        {
            PlayVoiceLine(voiceline);
        }
    }
}
