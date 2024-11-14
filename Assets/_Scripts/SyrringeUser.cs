using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SyrringeUser : MonoBehaviour
{
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private GameObject syrringeArms;

    [SerializeField] private int numSyrringes;
    [SerializeField] private int maxNumSyrringes = 5;
    [SerializeField] private float healAmount = 25;

    public int NumSyrringes { get { return numSyrringes; } set { SetSyrringes(value); } }
    public int MaxSyrringes { get { return maxNumSyrringes; } set { maxNumSyrringes = Mathf.Min(value, 0); } }

    private bool hasUsedSyrringe;

    [SerializeField] private PlayerInput playerInput;
    private InputAction action;
    private string key = "<nope>";
    private Health health;

    // Start is called before the first frame update
    void OnEnable()
    {
        action = playerInput.actions.FindAction("H");
        key = "[" + action.bindings.First().ToDisplayString() + "]";
        action.performed += Action_performed;

        if (!TryGetComponent(out Health health))
            Debug.LogError("SyrringeUser - No health script on player?");

        this.health = health;
    }

    private void OnDisable()
    {
        action.performed -= Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (numSyrringes > 0 && health.Value < health.MaxHealth)
        {
            weaponHandler.HideWeapons(()=>StartThrow());
        }
    }

    public void OnInteract()
    {
        Action_performed(new InputAction.CallbackContext());
    }

    private void StartThrow()
    {
        syrringeArms.SetActive(true);
    }

    /// <summary>
    /// ONLY CALL FROM SYRRINGEANIMATIONCALLBACK
    /// </summary>
    public void UseSyrringe()
    {
        if (!TryGetComponent(out Health health))
            return;

        numSyrringes--;
        hasUsedSyrringe = true;
        health.Heal(healAmount);
    }

    public void SetSyrringes(int value)
    {
        numSyrringes = Mathf.Clamp(value, 0, maxNumSyrringes);
        if (!hasUsedSyrringe && numSyrringes > 0)
        {
            TooltipUtil.Display("Press " + key + " to heal!", 5);
            hasUsedSyrringe = true;
        }
    }

    /// <summary>
    /// ONLY CALL FROM SYRRINGEANIMATIONCALLBACK
    /// </summary>
    public void UseFinished()
    {
        syrringeArms.SetActive(false);
        weaponHandler.UnideWeapons();
    }
}
