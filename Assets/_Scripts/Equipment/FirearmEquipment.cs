using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirearmEquipment : Equipment
{
    
    public static bool autoReload = true;
    public static bool holdToAds = true;
    
    [SerializeField] private FirearmData firearmData;
    [SerializeField] private int magCount = 30;
    bool canFire = false;
    bool reloding = false;
    bool wantReload = false;
    bool triggerHeld = false;
    float shootTimer = 0;
    AmmoPool pool;
    EquipmentList equipmentList;
    private bool CanFire => canFire && !reloding && magCount > 0;

    //public event Action OnFire;
    //public event Action OnNoAmmoLeft;

    void Awake()
    {
        input ??= new PlayerControls();
        input.Player.Fire.performed += Fire;
        input.Player.Fire.canceled += Fire;
        input.Player.Reload.performed += Reload;
        //input.Player.Reload.canceled += Reload;
        input.Player.SecondaryUse.performed += Ads;
        input.Player.SecondaryUse.canceled += Ads;
    }


    public override void OnInit()
    {
        pool = GetComponentInParent<AmmoPool>();
        equipmentList = GetComponentInParent<EquipmentList>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (CanFire)
        {
            if (triggerHeld) Shoot(dt);

        }

        if (wantReload)
        {
            
            DoReload();
        }

        shootTimer -= dt;
        if (shootTimer < 0) shootTimer = 0;
    }

    private void Shoot(float dt)
    {   
        if (shootTimer > 0) return;
        shootTimer = firearmData.FireTime;
        FireShot();

        if(magCount == 0 && autoReload)
        {
            wantReload = true;
        }
    }

    private void FireShot()
    {
        animator.SetTrigger("Shoot");
        magCount--;
    }


    private void Fire(InputAction.CallbackContext obj)
    {
        //Debug.Log("Fire performed: " + obj.performed + ", canceled: " + obj.canceled);
        //animator.SetBool("FireBool", obj.performed);
        triggerHeld = obj.performed;
    }
    private void Reload(InputAction.CallbackContext obj)
    {
        wantReload = true;

        //animator.SetBool("ReloadBool", obj.performed);
    }

    public void DoReload()
    {
        wantReload = false;
        //there is no ammo to take from the pool
        if (pool && !pool.HasAmmo(firearmData.ammoType))
        {
            //OnNoAmmoLeft?.Invoke();
            equipmentList.SelectWeapon();
            return;
        }

        if (magCount == 0)
        {
            animator.SetTrigger("ReloadEmpty");
            StartCoroutine(Reloading(1.9f));
        }
        else if (magCount < firearmData.MaxAmmo)
        {
            animator.SetTrigger("Reload");
            StartCoroutine(Reloading(1f));
        }

    }

    IEnumerator Reloading(float t)
    {
        reloding = true;
        yield return new WaitForSeconds(t);
        MagIn();
    }

    IEnumerator ReloadingShells(float t)
    {
        yield return new WaitForSeconds(t);
        ShellIn();
    }

    public override void BeginRaise()
    {
        reloding = false;
        canFire = true;
    }



    public void MagIn()
    {
        //my math aint mathing
        int ammoNeeded = firearmData.MaxAmmo - magCount;
        if(firearmData.canChamberRound && magCount == 0)
        {
            ammoNeeded = firearmData.magSize;
        }
        if (pool)
        {
            magCount += pool.TakeAmmo(firearmData.ammoType, ammoNeeded);
        }
        else
        {
            magCount = firearmData.MaxAmmo;
        }
        reloding = false;
    }

    public void ShellIn()
    {
        int ammoNeeded = 1;
        magCount += pool.TakeAmmo(firearmData.ammoType, ammoNeeded);
        if(magCount == firearmData.MaxAmmo)
        {
            reloding = false;
            //play some anim
        }
        else
        {
            StartCoroutine(ReloadingShells(.5f));
        }
    }

    private void Ads(InputAction.CallbackContext obj)
    {

    }
}
