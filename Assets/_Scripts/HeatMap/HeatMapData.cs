using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeatMapData : MonoBehaviour
{
    [SerializeField] private Player player;
    //List<Vector3> positions;
    //List<float> timeStamps;
    List<DataContainer> dataContainers;
    float timeStamp;

    private void Awake()
    {
        dataContainers = new List<DataContainer>();
    }

    void Start()
    {
        InvokeRepeating("RecordData", 0f, 1f);
    }

    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveDataToJson();
    }

    void RecordData()
    {
        timeStamp = Time.realtimeSinceStartup;

        DataContainer container = new DataContainer
        {
            playerPos = player.HeadPos,
            timeStamp = timeStamp
        };

        dataContainers.Add(container);
    }

    private void SaveDataToJson()
    {
        string json = JsonUtility.ToJson(new Wrapper(dataContainers), true);

        File.WriteAllText(Path.GetDirectoryName(Application.dataPath) 
            + "/Logs/HeatMapSessionData/" + DateTime.Now.ToString("yyyy-MM-dd_HH;mm;ss") + ".json", json);
    }

    [System.Serializable]
    private class DataContainer
    {
        public Vector3 playerPos;
        public float timeStamp;
    }

    [System.Serializable]
    private class Wrapper
    {
        public DataContainer[] dataContainers;

        public Wrapper(List<DataContainer> dataContainers)
        {
            this.dataContainers = new DataContainer[dataContainers.Count];

            for(int i = 0; i < dataContainers.Count; i++)
            {
                this.dataContainers[i] = dataContainers[i];
            }
        }
    }
}
