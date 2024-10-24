using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]private Animator animator;
    public UnityEvent throwEvent;

    [SerializeField] WeaponHandler weaponHandler;

    int throwHash = Animator.StringToHash("FireBool");
    int raiseHash = Animator.StringToHash("Flares.Raise");
    int extra = Animator.StringToHash("FireBool");

    // Start is called before the first frame update
    void Start()
    {
        if(!weaponHandler) weaponHandler = GetComponentInParent<WeaponHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Switch()
    {
        if(weaponHandler.EquippedGun != null)
            weaponHandler.EquippedGun.Switch();
    }

    public void Throw()
    {
        throwEvent?.Invoke();
    }

    public void StartAnimation()
    {
        
    }
}
