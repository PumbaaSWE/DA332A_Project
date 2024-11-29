using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class WeaponHandler : MonoBehaviour
{
    //public Dictionary<Cartridgetype, int> AmmoPool = new();
    public AmmoPool AmmunitionPool;
    public List<Firearm> Guns;
    public Firearm EquippedGun;
    //public bool DebugTest;
    public Transform FirearmRoot;
    [SerializeField] int maxGuns = 2;
    public UnityEvent OnShoot;
    public UnityEvent OnSwitchStart;
    public UnityEvent OnSwitchEnd;
    public UnityEvent OnReloadStart;
    public UnityEvent OnReloadEnd;

    public int MaxGuns => maxGuns;
    public bool CanPickup = true;

    private void Awake()
    {
        if (!TryGetComponent(out AmmunitionPool))
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

        if (EquippedGun)
            EquippedGun.Equip();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EquipGun(Firearm gunToEquip)
    {
        EquippedGun = gunToEquip;
        EquippedGun.Equip();
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

    /// <returns>Current Hipfire angle of the equipped gun</returns>
    public float GetHipfireAngle()
    {
        if (EquippedGun == null)
            return 0;

        return EquippedGun.HipFireSpread;
    }

    /// <returns>How much the equipped gun's sights are aimed down</returns>
    public float GetAdsProcentage()
    {
        if (EquippedGun == null)
            return 0;

        return EquippedGun.AdsProcentage;
    }

    /// <returns>Is the player aiming down sights</returns>
    public bool IsAds()
    {
        if (EquippedGun == null)
            return false;

        return EquippedGun.Ads;
    }

    public void CycleWeapons(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed && Guns.Count > 0 && Time.timeScale > 0)
            SwitchGun((Guns.IndexOf(EquippedGun) + 1) % Guns.Count);
    }

    /// <summary>
    /// Hides the curren weapon. Used when switching guns
    /// </summary>
    public void HideWeapons(Action onHide)
    {
        if (EquippedGun)
        {
            EquippedGun.Unequip(onHide);
        }
        else
        {
            onHide?.Invoke();
        }
    }

    /// <summary>
    /// Enables the currently equipped gun. Used when switching guns
    /// </summary>
    public void UnHideWeapons()
    {
        if (EquippedGun)
            EquippedGun.Equip();
    }

    public void SwitchGun(int gun)
    {
        if (gun < Guns.Count && Guns[gun] != EquippedGun && Guns[gun] != null)
        {
            EquippedGun.Unequip(() =>
            {
                EquipGun(Guns[gun]);
                OnSwitchEnd.Invoke();
            });

            OnSwitchStart.Invoke();
        }
    }

    public bool PickupGun(Firearm newGun, int loadedAmmo)
    {
        //if (Guns.Any(gun => newGun.name == gun.name))
        //    return false;

        if (!CanPickup || Time.timeScale == 0)
            return false;

        // If player has the same gun, grab the guns ammo
        if (HasGun(newGun.Id))
        {
            //AmmunitionPool[newGun.AmmoType] += loadedAmmo;
            return false;
        }

        Firearm gun = Instantiate(newGun.gameObject, FirearmRoot).GetComponent<Firearm>();
        gun.Set(this, GetComponent<RecoilHandler>(), GetComponent<MovementController>());
        gun.LoadedAmmo = Mathf.Clamp(loadedAmmo, 0, gun.MagazineSize + Convert.ToInt32(gun.RoundInTheChamber));
        gun.gameObject.SetActive(false);

        // Replaces the current gun and drops it
        if (Guns.Count == maxGuns)
            DropGun(gun);

        // Equip gun immediatly if player has no other guns
        else if (Guns.Count == 0)
        {
            Guns.Add(gun);
            EquipGun(gun);
        }

        // Switch to new gun
        else
        {
            Guns.Add(gun);
            SwitchGun(Guns.IndexOf(gun));
        }

        return true;
    }

    public void Shoot(CallbackContext context)
    {
        if (EquippedGun != null && Time.timeScale > 0)
        {
            EquippedGun.Shoot(context);
        }
    }

    public void AimDownSights(CallbackContext context)
    {
        if (EquippedGun != null && Time.timeScale > 0)
            EquippedGun.AimDownSights(context);
    }

    public void ToggleFireMode(CallbackContext context)
    {
        if (EquippedGun != null && Time.timeScale > 0)
            EquippedGun.ToggleFireMode(context);
    }

    public void Reload(CallbackContext context)
    {
        if (EquippedGun != null && Time.timeScale > 0)
        {
            EquippedGun.Reload(context);
            OnReloadStart.Invoke();
        }
    }

    public bool IsFiring()
    {
        if (EquippedGun == null)
            return false;

        return EquippedGun.Firing;
    }

    void DropGun()
    {
        EquippedGun.Unequip(() =>
        {
            GameObject droppedGun = Instantiate(EquippedGun.DropPrefab, FirearmRoot.position + FirearmRoot.forward, Quaternion.identity);
            droppedGun.GetComponent<FirearmPickUp>().LoadedAmmo = EquippedGun.LoadedAmmo;
            Destroy(EquippedGun.gameObject);
        });
    }

    void DropGun(Firearm newGun)
    {
        EquippedGun.Unequip(() =>
        {
            GameObject droppedGun = Instantiate(EquippedGun.DropPrefab, FirearmRoot.position + FirearmRoot.forward, Quaternion.identity);
            droppedGun.GetComponent<FirearmPickUp>().LoadedAmmo = EquippedGun.LoadedAmmo;
            Destroy(EquippedGun.gameObject);
            EquipGun(newGun);
        });
    }

    public bool HasGun()
    {
        return Guns.Count > 0;
    }

    public bool HasGun(int id)
    {
        return Guns.Any(x => x.Id == id);
    }

    public void SetCanFire(bool newValue)
    {
        if (EquippedGun != null)
            EquippedGun.SetCanFire(Convert.ToInt32(newValue));
    }

    public void SetCanAds(bool newValue)
    {
        if (EquippedGun != null)
            EquippedGun.SetCanFire(Convert.ToInt32(newValue));
    }

    public void SetCanPickup(bool newValue)
    {
        CanPickup = newValue;
    }
}

public enum Cartridgetype
{
    Rifle,
    Pistol,
    ShotgunShell
}