using UnityEngine;

[RequireComponent((typeof(AudioSource)), typeof(AudioLowPassFilter))]
public class OccludedAudioSource : MonoBehaviour
{
    [SerializeField] float occludedCutOffFreq = 1000f;
    [SerializeField] float defaultCutOffFreq = 22000f;
    [SerializeField] float lerpSpeed = 2f;
    [SerializeField] float occludedVolumeScale = 0.5f;
    [SerializeField] Transform nonOcclusionZone;

    bool occluded;
    float t;
    float defaultVolume;
    AudioLowPassFilter filter;
    AudioSource source;
    Transform player;
    Collider[] colliders;

    void Start()
    {
        filter = GetComponent<AudioLowPassFilter>();
        source = GetComponent<AudioSource>();

        defaultVolume = source.volume;

        if (nonOcclusionZone == null)
            return;

        colliders = nonOcclusionZone.GetComponentsInChildren<Collider>();

        foreach(var collider in colliders)
            collider.isTrigger = true;
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
        Vector3 dir = player.position - pos;
        float dist = dir.magnitude;

        bool hit = Physics.Raycast(pos, dir, out RaycastHit hitInfo, dir.magnitude, Physics.AllLayers, QueryTriggerInteraction.Ignore);


        occluded = hit && hitInfo.transform != player;

        if (occluded && colliders != null)
        {
            foreach (var collider in colliders)
            {
                Debug.Log("checking collider");
                if (collider.bounds.Contains(player.position))
                {
                    Debug.Log("not occluded!");
                    occluded = false;
                    break;
                }
            }
        }

        t = Mathf.Clamp01(t + Time.fixedDeltaTime * (occluded ? lerpSpeed : -lerpSpeed));

        // keeps updating volume in case it changes in runtime, might be unnecessary
        if (t == 0)
            defaultVolume = source.volume;

        filter.cutoffFrequency = Mathf.Lerp(defaultCutOffFreq, occludedCutOffFreq, t);
        source.volume = defaultVolume * Mathf.Lerp(1, occludedVolumeScale, t);
    }
}
