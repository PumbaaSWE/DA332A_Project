using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenericDoor : MonoBehaviour
{
    [SerializeField] Vector3 open;
    [SerializeField] Vector3 closed;
    [SerializeField] bool isOpen;
    [SerializeField] float duration;
    [SerializeField] AudioClips audioClips;
    [SerializeField] bool collideWithPlayer;
    AudioSource source;

#if UNITY_EDITOR
    [MakeButton("Set Open Rotation")]
    void SetOpen()
    {
        EditorUtility.SetDirty(this);
        open = transform.localRotation.eulerAngles;
    }

    [MakeButton("Set Closed Rotation")]
    void SetClosed()
    {
        EditorUtility.SetDirty(this);
        closed = transform.localRotation.eulerAngles;
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

    Quaternion startRot;
    Quaternion endRot;
    Vector3 dir;
    float timer;

    BoxCollider[] colliders;
    NonphysController player;

    void Start()
    {
        transform.localRotation = isOpen ? Quaternion.Euler(open) : Quaternion.Euler(closed);

        colliders = GetComponentsInChildren<BoxCollider>();
        enabled = false;

        source = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        isOpen = !isOpen;
        
        if (isOpen)
        {
            startRot = Quaternion.Euler(closed);
            endRot = Quaternion.Euler(open);
            PlayClip(audioClips.OpenStart);
        }
        else
        {
            startRot = Quaternion.Euler(open);
            endRot = Quaternion.Euler(closed);
            PlayClip(audioClips.CloseStart);
        }

        Quaternion rotation = Quaternion.Inverse(startRot) * endRot;
        rotation.ToAngleAxis(out float _, out Vector3 axis);
        dir = Vector3.Cross(new Vector3(axis.x == 1 ? 0 : 1, axis.y == 1 ? 0 : 1, axis.z == 1 ? 0 : 1), endRot.eulerAngles - startRot.eulerAngles);
        dir.Normalize();

        float biggestAngle = Quaternion.Angle(startRot, endRot);
        float currentAngle = Quaternion.Angle(transform.localRotation, endRot);

        timer = duration - currentAngle / biggestAngle * duration;
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
                if (collider.isTrigger)
                    continue;

                // Closest point on door from player
                Vector3 closestPointOnDoor = collider.ClosestPoint(player.transform.position + Vector3.up * player.CurrentHeight / 2f);

                // Check distance from player
                if (closestPointOnDoor.y >= player.transform.position.y - 0.1f &&
                    closestPointOnDoor.y < player.transform.position.y + player.CurrentHeight + 0.1f &&
                    Vector3.Distance(closestPointOnDoor.WithY(0), player.transform.position.WithY(0)) < player.Radius + 0.1f)
                {
                    // Check direction to player
                    Vector3 worldRotDir = transform.TransformDirection(dir);

                    Vector3 closestPointOnPlayer = player.Collider.ClosestPoint(closestPointOnDoor);

                    Vector3 doorToPlayer = (closestPointOnPlayer - closestPointOnDoor).normalized;

                    Debug.DrawRay(closestPointOnDoor, worldRotDir, Color.green);
                    Debug.DrawRay(closestPointOnDoor, doorToPlayer, Color.red);

                    if (Vector3.Dot(worldRotDir, doorToPlayer) > 0.5f)
                        return;
                }
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

        transform.localRotation = Quaternion.Slerp(startRot, endRot, timer / duration);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;


        if (source) {
            source.clip = clip;
            source.Play();
            return;
        } 

        var go = new GameObject();
        go.transform.position = transform.position;
        AudioSource source2 = go.AddComponent<AudioSource>();
        source2.clip = clip;
        source2.Play();
        Destroy(go, clip.length);
    }
}
