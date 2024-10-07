using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceData : MonoBehaviour
{
    public AudioSource source;
    public bool IsPlaying => source.isPlaying;
    public float TimeLeft => clipLength - source.time;
    public int Priority { get; set; }
    private float clipLength = 0;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1.0f;
    }

    public void Play(AudioClip clip, Vector3 position, int prio = 0)
    {
        transform.parent = null;
        transform.position = position;
        Priority = prio;
        source.volume = 1;
        source.pitch = 1;
        source.clip = clip; 
        source.Play();
        clipLength = clip.length;
    }

    public void PlayOn(AudioClip clip, Transform transform, int prio = 0)
    {
        transform.parent = transform;
        Priority = prio;
        source.volume = 1;
        source.pitch = 1;
        source.clip = clip;
        source.Play();
        clipLength = clip.length;
    }
}
