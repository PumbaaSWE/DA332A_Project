using UnityEngine;

[RequireComponent((typeof(AudioSource)), typeof(AudioLowPassFilter))]
public class OccludedAudioSource : MonoBehaviour
{
    [SerializeField] float occludedCutOffFreq = 1000f;
    [SerializeField] float defaultCutOffFreq = 22000f;
    [SerializeField] float lerpSpeed = 2f;

    bool occluded;
    float t;
    AudioLowPassFilter filter;
    Transform player;

    void Start()
    {
        filter = GetComponent<AudioLowPassFilter>();
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>().transform;

            if (player == null)
                return;
        }

        Vector3 pos = transform.position;
        Vector3 dir = player.transform.position - pos;
        float dist = dir.magnitude;

        bool hit = Physics.Raycast(pos, dir, out RaycastHit hitInfo, dir.magnitude, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        occluded = hit && hitInfo.transform != player;

        t = Mathf.Clamp01(t + Time.fixedDeltaTime * (occluded ? lerpSpeed : -lerpSpeed));
        filter.cutoffFrequency = Mathf.Lerp(defaultCutOffFreq, occludedCutOffFreq, t);
    }
}
