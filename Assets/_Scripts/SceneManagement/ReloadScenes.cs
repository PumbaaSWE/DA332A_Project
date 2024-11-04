using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadScenes : MonoBehaviour
{ 
    public void Reload()
    {
        SceneGroupLoader.Instance.ReloadLast();
    }
}
