using UnityEngine;

public class DetectableTarget : MonoBehaviour
{

    public bool bosted = false;
    void Start()
    {
        DetectableTargetManager.Instance.Register(this);
    }



    private void OnDisable()
    {
        
        DetectableTargetManager.Instance.Deregister(this);
        
    }
}
