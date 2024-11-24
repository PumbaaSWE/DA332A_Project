using System.Collections;

using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    public bool bosted = false;
    //void Start()
    //{
    //    DetectableTargetManager.Instance.Register(this);
    //}

    private IEnumerator WaitForManagerAndRegister()
    {
        while (DetectableTargetManager.Instance == null)
        {
            yield return null; 
        }

        DetectableTargetManager.Instance.Register(this);
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForManagerAndRegister());
    }

    private void OnDisable()
    {
        if (DetectableTargetManager.Instance != null)
        {
            DetectableTargetManager.Instance.Deregister(this);
        }
    }

   
}
