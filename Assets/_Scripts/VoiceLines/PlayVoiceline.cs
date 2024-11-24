using System.Collections;
using UnityEngine;

public class PlayVoiceline : MonoBehaviour
{
    [SerializeField] private VoicelineData voiceline;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool disableOnPlay = false;
    [SerializeField] private float delay = 1;

    [SerializeField] private VoicelineData[] voicelines;

    public void Play()
    {
        if (!this.enabled) {  return; }
        if(voiceline)
            VoicelineManager.Instance.QueueVoiceLine(voiceline);
        if(voicelines != null)
        {
            for (int i = 0; i < voicelines.Length; i++)
            {
                VoicelineManager.Instance.QueueVoiceLine(voicelines[i]);
            }
        }
        if (disableOnPlay)
        {
            this.enabled = false;
        }
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
