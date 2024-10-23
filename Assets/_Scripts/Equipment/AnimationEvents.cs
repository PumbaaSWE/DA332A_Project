using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]private Animator animator;
    public UnityEvent throwEvent;

    int throwHash = Animator.StringToHash("FireBool");
    int raiseHash = Animator.StringToHash("Flares.Raise");
    int extra = Animator.StringToHash("FireBool");

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

    public void StartAnimation()
    {
        
    }
}
