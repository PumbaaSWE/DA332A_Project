using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    public bool bosted = false;
    void Start()
    {
        DetectableTargetManager.Instance.Register(this);
    }



    private void OnDestroy()
    {
        if (DetectableTargetManager.Instance != null)
        {
            DetectableTargetManager.Instance.Deregister(this);
        }
    }
}
