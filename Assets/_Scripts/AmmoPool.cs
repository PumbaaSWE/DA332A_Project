using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoPool : MonoBehaviour
{
    public List<CartridgePool> Pools = new();


    private void Awake()
    {
        Pools = new()
        {
            new CartridgePool()
            {
                AmmoType = Cartridgetype.Rifle,
                CurrentAmmo = 0,
                MaxAmmo = 120
            },
            new CartridgePool()
            {
                AmmoType = Cartridgetype.ShotgunShell,
                CurrentAmmo = 0,
                MaxAmmo = 60
            },
            new CartridgePool()
            {
                AmmoType = Cartridgetype.Pistol,
                CurrentAmmo = 0,
                MaxAmmo = 90
            },
        };

    }

    public void Add(Cartridgetype type, int start, int max)
    {
        Pools.Add(new()
        {
            AmmoType = type,
            CurrentAmmo = start,
            MaxAmmo = max
        });
    }

    public int this[Cartridgetype type, bool maxAmmo = false]
    {
        get
        {
            if (maxAmmo)
                return Pools.Find(p => p.AmmoType == type).MaxAmmo;

            return Pools.Find(p => p.AmmoType == type).CurrentAmmo;
        }
        set
        {
            if (maxAmmo)
                Pools.Find(p => p.AmmoType == type).MaxAmmo = value;
            
            else
                Pools.Find(p => p.AmmoType == type).CurrentAmmo = value;
        }
    }

    public bool ContainsKey(Cartridgetype type)
    {
        return Pools.Any(p => p.AmmoType == type);
    }
}

[Serializable]
public class CartridgePool
{
    public Cartridgetype AmmoType;
    public int CurrentAmmo;
    public int MaxAmmo;
}