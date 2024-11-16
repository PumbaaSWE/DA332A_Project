using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    public bool bosted = false;
    void Start()
    {
        DetectableTargetManager.Instance.OrNull()?.Register(this);
    }



    private void OnDisable()
    {
        
        DetectableTargetManager.Instance.OrNull()?.Deregister(this);
        
    }
}
