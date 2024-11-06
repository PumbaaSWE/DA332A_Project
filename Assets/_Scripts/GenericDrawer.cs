using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenericDrawer : MonoBehaviour
{
    [SerializeField] Vector3 open;
    [SerializeField] Vector3 closed;
    [SerializeField] bool isOpen;
    [SerializeField] float duration;
    [SerializeField] AudioClips audioClips;
    [SerializeField] bool collideWithPlayer;

#if UNITY_EDITOR
    [MakeButton("Set Open Position")]
    void SetOpen()
    {
        EditorUtility.SetDirty(this);
        open = transform.localPosition;
    }
    [MakeButton("Set Closed Position")]
    void SetClosed()
    {
        EditorUtility.SetDirty(this);
        closed = transform.localPosition;
    }
#endif

    [Serializable]
    public struct AudioClips
    {
        public AudioClip OpenStart;
        public AudioClip OpenEnd;
        public AudioClip CloseStart;
        public AudioClip CloseEnd;
    }

    Vector3 startPos;
    Vector3 endPos;
    float timer;

    BoxCollider[] colliders;
    NonphysController player;

    void Start()
    {
        transform.localPosition = isOpen ? open : closed;

        colliders = GetComponentsInChildren<BoxCollider>();
        enabled = false;
    }

    public void Interact()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            startPos = closed;
            endPos = open;
            PlayClip(audioClips.OpenStart);
        }
        else
        {
            startPos = open;
            endPos = closed;
            PlayClip(audioClips.CloseStart);
        }

        float biggestDistance = Vector3.Distance(startPos, endPos);
        float currentDistance = Vector3.Distance(transform.localPosition, endPos);

        timer = duration - currentDistance / biggestDistance * duration;
        enabled = true;
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<NonphysController>();

            if (player == null)
                return;
        }

        if (collideWithPlayer)
        {
            foreach (var collider in colliders)
            {
                Vector3 closestPoint = collider.ClosestPoint(player.transform.position + Vector3.up * player.CurrentHeight / 2f);

                float dot = Vector3.Dot(player.transform.position - closestPoint, transform.TransformPoint(endPos) - transform.TransformPoint(startPos));

                //Debug.DrawLine(closestPoint, player.transform.position + Vector3.up * player.CurrentHeight / 2f);
                //Debug.DrawRay(closestPoint, transform.TransformPoint(endPos) - transform.TransformPoint(startPos));
                //Debug.Log(dot);

                if (dot < 0)
                    continue;

                if (closestPoint.y >= player.transform.position.y - 0.1f &&
                    closestPoint.y < player.transform.position.y + player.CurrentHeight + 0.1f &&
                    Vector3.Distance(closestPoint.WithY(0), player.transform.position.WithY(0)) < player.Radius + 0.1f)
                    return;
            }
        }

        timer += Time.deltaTime;

        if (timer >= duration)
        {
            if (isOpen)
                PlayClip(audioClips.OpenEnd);
            else
                PlayClip(audioClips.CloseEnd);

            timer = duration;
            enabled = false;
        }

        transform.localPosition = Vector3.Lerp(startPos, endPos, timer / duration);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        var go = new GameObject();
        go.transform.position = transform.position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(go, clip.length);
    }
}
