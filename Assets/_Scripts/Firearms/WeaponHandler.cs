using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class WeaponHandler : MonoBehaviour
{
    //public Dictionary<Cartridgetype, int> AmmoPool = new();
    public AmmoPool AmmunitionPool;
    public List<Firearm> Guns;
    public Firearm EquippedGun;
    public bool DebugTest;
    public Transform FirearmRoot;
    [SerializeField] int MaxGuns;

    private void Awake()
    {
        if (!TryGetComponent<AmmoPool>(out AmmunitionPool))
        {
            gameObject.AddComponent<AmmoPool>();
            AmmunitionPool = GetComponent<AmmoPool>();
            AmmunitionPool.Add(Cartridgetype.Pistol, 0, 300);
            AmmunitionPool.Add(Cartridgetype.ShotgunShell, 0, 120);
            AmmunitionPool.Add(Cartridgetype.Rifle, 0, 400);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (DebugTest)
        //    foreach (Cartridgetype type in Enum.GetValues(typeof(Cartridgetype)))
        //        AmmunitionPool.Add(type, 1000);

        // Create Ammo pool Component if it doesn't already exist

        foreach (Firearm gun in Guns)
            gun.Set(this, GetComponent<RecoilHandler>(), GetComponent<MovementController>());
    }

    // Update is called once per frame
    void Update()
    {
        if (EquippedGun == null && Guns.Count > 0)
        {
            EquippedGun = Guns[0];
            EquippedGun.Equip();
        }
    }

    public int TakeAmmo(Cartridgetype type, int ammoToTake)
    {
        if (AmmunitionPool.ContainsKey(type))
        {
            int ammoToGive = Mathf.Clamp(AmmunitionPool[type], 0, ammoToTake);
            AmmunitionPool[type] -= ammoToGive;
            return ammoToGive;
        }

        else
            return 0;
    }

    public void AddAmmo(Cartridgetype type, int ammoToAdd)
    {
        //if (!AmmunitionPool.ContainsKey(type))
        //    AmmunitionPool.Add(type, ammoToAdd);

        //else
        AmmunitionPool[type] = Mathf.Min(AmmunitionPool[type] + ammoToAdd, AmmunitionPool[type, true]);
    }

    /// <returns>True if any ammo of type left, false if not</returns>
    public bool AmmoLeft(Cartridgetype type)
    {
        if (!AmmunitionPool.ContainsKey(type))
            return false;

        return AmmunitionPool[type] > 0;
    }

    /// <returns>Remaining ammo in magazine of current gun</returns>
    public int GetMagazineCount()
    {
        if (EquippedGun == null)
            return 0;

        return EquippedGun.LoadedAmmo;
    }

    /// <returns>Reserve ammo for current gun</returns>
    public int GetAmmoCount()
    {
        if (EquippedGun == null)
            return 0;

        if (!AmmunitionPool.ContainsKey(EquippedGun.AmmoType))
            return 0;

        return AmmunitionPool[EquippedGun.AmmoType];
    }

    public void CycleWeapons(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed && Guns.Count > 0)
            SwitchGun((Guns.IndexOf(EquippedGun) + 1) % Guns.Count);
    }

    public void SwitchGun(int gun)
    {
        if (gun < Guns.Count && Guns[gun] != EquippedGun && Guns[gun] != null)
        {
            EquippedGun.Unequip(() =>
            {
                Guns[gun].Equip();
                EquippedGun = Guns[gun];
            });
        }
    }

    public bool PickupGun(GameObject newGun, int loadedAmmo)
    {
        //if (Guns.Any(gun => newGun.name == gun.name))
        //    return false;

        // If player has the same gun, grab the guns ammo
        if (Guns.Any(gun => newGun.name == gun.name))
        {
            AmmunitionPool[newGun.GetComponent<Firearm>().AmmoType] += loadedAmmo;
            return true;
        }

        // Replace currently held gun
        else if (Guns.Count == MaxGuns)
        {
            int index = Guns.IndexOf(EquippedGun);

            DropGun();

            GameObject gun = Instantiate(newGun, FirearmRoot);
            Guns[index] = gun.GetComponent<Firearm>();
            EquippedGun = Guns[index];
        }

        // 
        else
        {
            GameObject gun = Instantiate(newGun, FirearmRoot);

            if (EquippedGun != null)
                EquippedGun.gameObject.SetActive(false);

            EquippedGun = gun.GetComponent<Firearm>();
            Guns.Add(EquippedGun);
        }

        EquippedGun.Set(this, GetComponent<RecoilHandler>(), GetComponent<MovementController>());
        EquippedGun.Equip();
        EquippedGun.LoadedAmmo = Mathf.Clamp(loadedAmmo, 0, EquippedGun.MagazineSize + Convert.ToInt32(EquippedGun.RoundInTheChamber));
        return true;
    }

    public void Shoot(CallbackContext context)
    {
        if (EquippedGun != null)
            EquippedGun.Shoot(context);
    }

    public void AimDownSights(CallbackContext context)
    {
        if (EquippedGun != null)
            EquippedGun.AimDownSights(context);
    }

    public void ToggleFireMode(CallbackContext context)
    {
        if (EquippedGun != null)
            EquippedGun.ToggleFireMode(context);
    }

    public void Reload(CallbackContext context)
    {
        if (EquippedGun != null)
            EquippedGun.Reload(context);
    }

    public bool IsFiring()
    {
        if (EquippedGun == null)
            return false;

        return EquippedGun.Firing;
    }

    void DropGun()
    {
        GameObject droppedGun = Instantiate(EquippedGun.DropPrefab, FirearmRoot.position + FirearmRoot.forward, Quaternion.identity);
        droppedGun.GetComponent<FirearmPickUp>().LoadedAmmo = EquippedGun.LoadedAmmo;
        Destroy(EquippedGun.gameObject);
    }
}

public enum Cartridgetype
{
    Rifle,
    Pistol,
    ShotgunShell
}