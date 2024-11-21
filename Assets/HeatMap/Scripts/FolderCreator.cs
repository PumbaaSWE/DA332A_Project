using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using System.IO;


public class FolderCreator : MonoBehaviour
{
    //[MenuItem("AssetDatabase/Create Folder")]

  public static void CreateNewFolder(string name)
  {
      
        Directory.CreateDirectory($"Assets/HeatMap/SessionData/{name}");
        AssetDatabase.Refresh();
        Debug.Log("creating folder");

  }


}
