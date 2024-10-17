using System;

[Serializable]
public class GameData
{
    public string name;
    public int id;
    public PlayerData playerData;
}
[Serializable]
public class PlayerData
{
    public float health;
    public int numFlares;
    public int numPistolRounds;
    public int numRifleRounds;
    public int numShotgunShells;
    public WeaponData[] weaponData;
    public int equippedWeapon;
}

[Serializable]
public struct WeaponData
{
    public int id;
    public int ammo;
}
