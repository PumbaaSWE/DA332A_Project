using UnityEngine;

public class PlayerInventoryData : ScriptableObject
{
    //public GameObject[] eqipmentPrefabs;
    //public Firearm[] firearms;
         
    //public int[] equpment;
    //public EquipmentCount[] equpmentData;

    public int flaresCount;
    public float health;
    
    public void BuildInventory(Transform player)
    {
        player.GetComponent<Health>().SetHealth(health);
        player.GetComponent<FlareThrower>().numFlares = flaresCount;
    }

    public string Save()
    {
        return JsonUtility.ToJson(this);
    }

    public void Load(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }


}

//[Serializable]
//public struct EquipmentCount
//{
//    public int count;
//    public int max;

//}

//[Serializable]
//public struct FirearmSaveData
//{
//    public int id;
//    public int ammo;
//    public int firemode;

//}
