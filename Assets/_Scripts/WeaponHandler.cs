using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class WeaponHandler : MonoBehaviour
{
    public Dictionary<Cartridgetype, int> AmmoPool = new();
    public List<Firearm> Guns;
    Firearm EquippedGun;
    public bool DebugTest;

    // Start is called before the first frame update
    void Start()
    {
        if (DebugTest)
            foreach (Cartridgetype type in Enum.GetValues(typeof(Cartridgetype)))
                AmmoPool.Add(type, 1000);
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
        if (AmmoPool.ContainsKey(type))
            return Mathf.Clamp(Mathf.Max(AmmoPool[type] -= ammoToTake, 0), 0, ammoToTake);

        else
            return 0;
    }

    public void AddAmmo(Cartridgetype type, int ammoToAdd)
    {
        if (!AmmoPool.ContainsKey(type))
            AmmoPool.Add(type, ammoToAdd);

        else
            AmmoPool[type] += ammoToAdd;
    }

    /// <returns>True if any ammo of type left, false if not</returns>
    public bool AmmoLeft(Cartridgetype type)
    {
        return AmmoPool[type] > 0;
    }

    public void CycleWeapons(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
            SwitchGun((Guns.IndexOf(EquippedGun) + 1) % Guns.Count);
    }

    public void SwitchGun(int gun)
    {
        if (gun < Guns.Count && Guns[gun] != EquippedGun && Guns[gun] != null)
        {
            EquippedGun.Unequip(() => Guns[gun].Equip());
            EquippedGun = Guns[gun];
        }
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
}

public enum Cartridgetype
{
    Rifle,
    Pistol,
    ShotgunShell
}