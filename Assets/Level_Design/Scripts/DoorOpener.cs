using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;

    AudioSource source;


    bool hasOpened = false;
    private void Start()
    {


    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && source != null && hasOpened)
        {
            source.clip = closeClip;
            source.Play();
            anim.SetTrigger("Exit");
            anim.ResetTrigger("Open");
            hasOpened = false;
        }
    }

    public void OpenDoor()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = openClip;
        source.Play();
        anim.SetTrigger("Open");
        hasOpened = true;
    }

    public void OpenDoor(Transform transform)
    {
        //Debug.Log(transform);
        if (!transform.gameObject.GetComponent<WeaponHandler>().enabled) 
        {
            return;
        }
        if (hasOpened)
        {
            return;
        }
        source = gameObject.AddComponent<AudioSource>();
        source.clip = openClip;
        source.Play();
        anim.SetTrigger("Open");
        hasOpened = true;
    }
}
