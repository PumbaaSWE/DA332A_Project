using JetBrains.Annotations;
using System;
using UnityEngine.SearchService;

[Serializable]
public class GameData
{
    public string name;
    public int id;
    public PlayerData playerData;
    public BlackboardData[] blackboardData;
}

[Serializable]
public struct BlackboardData
{
    public string key;
    public object value;
}

[Serializable]
public class PlayerData
{
    public float health;
    public int numFlares;
    public int numPistolRounds;
    public int numRifleFounds;
    public int numShutgunShells;
}

