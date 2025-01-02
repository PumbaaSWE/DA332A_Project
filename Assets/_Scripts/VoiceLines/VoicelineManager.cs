using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class VoicelineManager : Singleton<VoicelineManager>
{
    private AudioSource source;
    [SerializeField] private SubtitleChannel subtitleChannel;

    private readonly Queue<VoicelineData> queue = new();
    private float clipLength;
    float[] subtitleDuration;
    float[] timeStamps;
    string[] subtitles;
    Color subtitleColor;
    int voicelineSequence;
    bool paused;

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
        paused = false;
        clipLength = voicelineTime > 0.0f ? voicelineTime : clip.length;

        InitializeNewSubtitle(subtitleText, subtitleTime, subtitleColor);
    }

    void InitializeNewSubtitle(string[] subtitleText, float[] subtitleTime, Color subtitleColor)
    {
        
        voicelineSequence = 0;
        if (subtitleText == null || subtitleTime == null || subtitleText.Length == 0 || subtitleTime.Length != subtitleText.Length)
        {
            subtitles = new string[0];
            subtitleDuration = new float[0];
            return;
        }
        subtitleDuration = new float[subtitleTime.Length];
        timeStamps = new float[subtitleTime.Length];
        subtitles = new string[subtitleText.Length];
        float timeTotal = 0.0f;
        for (int i = 0; i < subtitleText.Length; i++)
        {
            subtitleDuration[i] = subtitleTime[i] - timeTotal;
            subtitles[i] = subtitleText[i];
            timeTotal += subtitleDuration[i];
            
        }

        for(int i = 1; i < timeStamps.Length; i++)
        {
            timeStamps[i] = subtitleTime[i - 1];
        }

        this.subtitleColor = subtitleColor;
        SetNewSubtitle(voicelineSequence);
    }

    public void Skip(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Skip();
        }
    }

    public void Skip()
    {
        if (!source.isPlaying) { return; }
        voicelineSequence++;
        if (subtitles.Length <= voicelineSequence) // play next audio in queue or stop
        {
            Stop();
            return;
        }
        SetNewSubtitle(voicelineSequence);
        source.time = timeStamps[voicelineSequence];
    }

    public void Stop() // stops current voiceline
    {
        clipLength = 0.0f;
        source.Stop();
        source.clip = null;
        if (subtitleChannel)
        {
            SubtitleData subtitle = new()
            {
                text = "",
                time = 0.0f,
                color = Color.white
            };
            subtitleChannel.Raise(subtitle);
        }
    }

    private void SetNewSubtitle(int i)
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

    void HandlePause()
    {
        if (source.isPlaying && Time.timeScale == 0 && !paused)
        {
            source.Pause();
            paused = true;
            return;
        }
        if (!source.isPlaying && Time.timeScale == 1 && paused)
        {
            paused = false;
        }
        if (clipLength > 0 && !source.isPlaying && !paused && source.clip != null) { source.UnPause(); }
    }

    public void Update()
    {
        if ((source.isPlaying && Time.timeScale == 0) || (!source.isPlaying && Time.timeScale == 1))
        {
            HandlePause();
        }
        if (paused) { return; }

        if (subtitles != null && subtitles.Length > voicelineSequence)
        {
            subtitleDuration[voicelineSequence] -= Time.deltaTime;

            if (subtitleDuration[voicelineSequence] <= 0)
            {
                voicelineSequence++;
                if(subtitles.Length > voicelineSequence)
                {
                    SetNewSubtitle(voicelineSequence);
                }
            }
        }
        if (source.isPlaying || clipLength > 0)
        {
            clipLength -= Time.deltaTime;
            return;
        }
        else
        {
            Stop();
        }

        if (queue.TryDequeue(out VoicelineData voiceline))
        {
            PlayVoiceLine(voiceline);
        }
    }
}
