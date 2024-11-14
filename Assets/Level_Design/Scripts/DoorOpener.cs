using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;

    AudioSource source;

    private void Start()
    {


    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && source != null)
        {
            source.clip = closeClip;
            source.Play();
            anim.SetTrigger("Exit");
        }
    }

    public void OpenDoor()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = openClip;
        source.Play();
        anim.SetTrigger("Open");
    }
}
