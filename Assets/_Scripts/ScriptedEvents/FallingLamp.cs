using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FallingLamp : MonoBehaviour
{
    [SerializeField] float toY = 0f;
    [SerializeField] float gravity = 9.81f;

    [SerializeField] Light lamp;
    [SerializeField] AudioClip[] playSound;
    [SerializeField] UnityEvent OnDone;

    Vector3 vel;

    void Start()
    {
        if (lamp == null)
            lamp = GetComponent<Light>();

        if (lamp == null)
            lamp = GetComponentInChildren<Light>();
    }

    void Update()
    {
        vel += Vector3.down * gravity * Time.deltaTime;
        transform.position += vel * Time.deltaTime;

        if (transform.position.y < toY)
        {
            transform.position = transform.position.WithY(toY);

            if (lamp != null)
            {
                lamp.enabled = false;
            }

            if (playSound != null)
            {
                foreach(var sound in playSound)
                    AudioSource.PlayClipAtPoint(sound, transform.position);
            }

            OnDone.Invoke();
            enabled = false;
        }
    }
}
