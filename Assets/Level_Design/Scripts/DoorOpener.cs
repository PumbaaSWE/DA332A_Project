using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;

    [SerializeField] AudioSource source;


    bool hasOpened = false;
    private void Start()
    {

        if(!source) source =  gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && hasOpened)
        {
            if (source) { 
                source.clip = closeClip;
                source.Play();
            }
            anim.SetTrigger("Exit");
            anim.ResetTrigger("Open");
            hasOpened = false;
        }
    }

    public void OpenDoor()
    {
        if (source)
        {
            source.clip = openClip;
            source.Play();
        }    
        anim.SetTrigger("Open");
        hasOpened = true;
    }

    public void OpenDoor(Transform transform)
    {
        //Debug.Log(transform);
        //if (!transform.gameObject.GetComponent<WeaponHandler>().enabled) 
        //{
        //    return;
        //}
        if (hasOpened)
        {
            return;
        }
        OpenDoor();
    }
}
