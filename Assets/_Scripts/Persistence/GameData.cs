using System;

[Serializable]
public class GameData
{
    public GameData()
    {
        playerData = new PlayerData();

        // TODO: determine default data. What should player get at start of the first level?
        // start weapon?
        // most important, health? start with max i guess, but where do we get it from?
    }

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
