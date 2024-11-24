using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    public bool bosted = false;
    //void Start()
    //{
    //    DetectableTargetManager.Instance.Register(this);
    //}

    private void OnEnable()
    {
        if (DetectableTargetManager.Instance != null)
        {
            DetectableTargetManager.Instance.Register(this);
        }
        else
        {
            Debug.LogError("DetectableTargetManager.Instance is null during OnEnable.");
        }
    }

    private void OnDisable()
    {
        
        DetectableTargetManager.Instance.Deregister(this);
        
    }
}
