using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public List<DataSensor> allData { get; private set; } = new List<DataSensor>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple DataManager found. Destroying" + gameObject.name);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Register(DataSensor sensor)
    {
        allData.Add(sensor);
    }

    public void Deregister(DataSensor sensor)
    {
        allData.Remove(sensor);
    }
    public void TransferData(GameObject sourse, Vector3 location)
    {

    }
}
