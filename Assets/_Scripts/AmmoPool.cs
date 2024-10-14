using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoPool : MonoBehaviour
{
    public List<CartridgePool> Pools;

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