using System.Collections;
using UnityEngine;

public class PlayVoiceline : MonoBehaviour
{
    [SerializeField] private VoicelineData voiceline;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private float delay = 1;

    [SerializeField] private VoicelineData[] voicelines;

    public void Play()
    {
        if(voiceline)
            SimpleVoiceManager.Instance.QueueVoiceLine(voiceline);
        if(voicelines != null)
        {
            for (int i = 0; i < voicelines.Length; i++)
            {
                SimpleVoiceManager.Instance.QueueVoiceLine(voicelines[i]);
            }
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
