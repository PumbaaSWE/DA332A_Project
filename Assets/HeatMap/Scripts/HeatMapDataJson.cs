using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeatMapDataJson : MonoBehaviour
{
    [SerializeField] private Player player;
    //List<Vector3> positions;
    //List<float> timeStamps;
    List<DataContainer> dataContainers;
    float timeStamp;
    string folderName;
    string levelName;

    private void Awake()
    {
        dataContainers = new List<DataContainer>();
    }

    void Start()
    {
        Debug.Log("Data Tracking Started");

        InvokeRepeating("RecordData", 0f, 0.125f);
        folderName = DateTime.Now.ToString("yyyy-MM-dd_HH;mm;ss");
        FolderCreator.CreateNewFolder(folderName);
        levelName = "Level0"; //make this to read from Scene Level name
    }

    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveDataToJson();

        Debug.Log("Saved");
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

        //Debug.Log("Data saved: " + container.playerPos);
    }

    private void SaveDataToJson()
    {

        string json = JsonUtility.ToJson(new Wrapper(dataContainers), true);
        Debug.Log(json + " Json stored");

        File.WriteAllText(Path.GetDirectoryName(Application.dataPath) 
            + "/Assets/HeatMap/SessionData/" + folderName + "/" + levelName + ".json", json);
    }

    [System.Serializable]
    public class DataContainer
    {
        public Vector3 playerPos;
        public float timeStamp;
    }

    [System.Serializable]
    public class Wrapper
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
