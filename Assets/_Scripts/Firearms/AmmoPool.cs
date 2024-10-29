using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoPool : MonoBehaviour
{
    public List<CartridgePool> Pools = new();

    Dictionary<AmmoType, AmmoStash> pool = new();

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

    public int TakeAmmo(AmmoType type, int amount)
    {
        if(pool.TryGetValue(type, out AmmoStash stash))
        {
            int canTake = Mathf.Min(stash.currentAmount, amount);
            stash.currentAmount -= canTake;
            return canTake;
        }
        return amount;
    }

    public int AddAmmo(Cartridgetype type, int amount)
    {
        CartridgePool cp = Pools.Find(p => p.AmmoType == type);

        int newAmount = cp.CurrentAmmo + amount;
        int leftOver = 0;
        if(newAmount > cp.MaxAmmo)
        {
            leftOver = newAmount - cp.MaxAmmo;
            newAmount = cp.MaxAmmo;
        }

        cp.CurrentAmmo = newAmount;
        return leftOver;
    }


    public void AddAmmo(AmmoType type, int amount)
    {
        if (pool.TryGetValue(type, out AmmoStash stash))
        {
            stash.currentAmount += amount;
            stash.currentAmount = Mathf.Min(stash.maxAount, stash.currentAmount);
        }
        else
        {
            pool.Add(type, new AmmoStash());
        }
    }

    public bool HasAmmo(AmmoType type)
    {
        if (pool.TryGetValue(type, out AmmoStash stash))
        {
            return stash.currentAmount > 0;
        }
        return true;
    }
    public int CurrentAmmo(AmmoType type)
    {
        if (pool.TryGetValue(type, out AmmoStash stash))
        {
            return stash.currentAmount;
        }
        return -1;
    }

    public bool ContainsKey(Cartridgetype type)
    {
        if (Pools == null)
            return false;

        return Pools.Any(p => p.AmmoType == type);
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
}

[Serializable]
public class CartridgePool
{
    public Cartridgetype AmmoType;
    public int CurrentAmmo;
    public int MaxAmmo;
}

[Serializable]
public class AmmoStash
{
    public int currentAmount = 100;
    public int maxAount = 100;
}