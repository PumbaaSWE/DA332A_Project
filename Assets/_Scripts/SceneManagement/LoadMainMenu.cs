using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    [SerializeField] SceneField scene;

    public void Load()
    {
        if (scene == null)
            return;

        FindObjectOfType<SceneGroupManager>().UnloadScenes();
        SceneManager.LoadScene(scene.SceneName);
    }
}
