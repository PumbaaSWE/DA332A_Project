using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveLevelProgress", menuName = "ScriptableObjects/SaveLevelProgress", order = 5)]
public class SaveLevelProgress : ScriptableObject
{

    private readonly string fileName = "LevelData.txt";
    private string dataPath;

    [SerializeField]private int level = -1;

    public int Level
    {
        get { return level; }
        set { 
            level = value;
            Save();
        }
    }

    public void Save()
    {
        dataPath = Application.persistentDataPath;
        string fullPath = Path.Combine(dataPath, fileName);
        try
        {
            //if (!File.Exists(fullPath))
            //{
            //    File.Create(fullPath);
            //}
            string data = JsonUtility.ToJson(this, true);
            File.WriteAllText(fullPath, data);
        }
        catch(Exception e)
        {
            Debug.LogError("Failed file: " + e.StackTrace);
        }
        

    }

    public void Load()
    {
        dataPath = Application.persistentDataPath;
        string fullPath = Path.Combine(dataPath, fileName);

        
        try {
            if (File.Exists(fullPath))
            {
                string data = JsonUtility.ToJson(this, true);
                File.WriteAllText(fullPath, data);
            }
            else
            {
                level = -1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed file: " + e.StackTrace);
        }
    }

    void OnEnable()
    {
        Load();
    }


}
