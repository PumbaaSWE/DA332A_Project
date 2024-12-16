using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckIfPickedUp : MonoBehaviour
{
    [SerializeField] GameObject go1;
    [SerializeField] GameObject go2;

    public UnityEvent<Transform> OnDone;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Check()
    {
        if (!go1.activeSelf && !go2.activeSelf)
        {
            OnDone.Invoke(transform);
        }
    }
}
