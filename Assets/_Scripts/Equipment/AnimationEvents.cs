using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{

    public UnityEvent throwEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Throw()
    {
        throwEvent?.Invoke();
    }
}
