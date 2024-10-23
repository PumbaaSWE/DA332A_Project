using UnityEngine;
using UnityEngine.InputSystem;

public class FirearmEquipment : Equipment
{

    void Awake()
    {
        input ??= new PlayerControls();
        input.Player.Fire.performed += Fire;
        input.Player.Fire.canceled += Fire;
        input.Player.Reload.performed += Reload;
        input.Player.Reload.canceled += Reload;
        input.Player.SecondaryUse.performed += Ads;
        input.Player.SecondaryUse.canceled += Ads;
    }



    private void Fire(InputAction.CallbackContext obj)
    {
        Debug.Log("Fire performed: " + obj.performed + ", canceled: " + obj.canceled);
        animator.SetBool("FireBool", obj.performed);
    }
    private void Reload(InputAction.CallbackContext obj)
    {
        Debug.Log("Reload performed: " + obj.performed + ", canceled: " + obj.canceled);
        animator.SetBool("ReloadBool", obj.performed);
    }
    private void Ads(InputAction.CallbackContext obj)
    {

    }
}
