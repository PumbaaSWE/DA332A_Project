using JetBrains.Annotations;
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
        // i will hardcode some stuff for now, because spawning empty player is annoying

        playerData.health = 100;

        playerData.numFlares = 5;
        playerData.numPistolRounds = 90;
        playerData.numRifleRounds = 120;
        playerData.numShotgunShells = 40;

        playerData.weaponData = new WeaponData[]
        {
            new WeaponData()
            {
                id = 0,
                ammo = 15,
            },
            new WeaponData()
            {
                id = 1,
                ammo = 20
            }
        };

        playerData.equippedWeapon = 1;
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
