using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadScene : MonoBehaviour
{
    public SceneField scene;
    string sceneName;
    bool useString;

    public void Unload()
    {
        useString = false;
        StartCoroutine(UnloadCoroutine());
    }

    public void Unload(Scene scene)
    {
        useString = true;
        sceneName = scene.name;
        StartCoroutine(UnloadCoroutine());
    }

    IEnumerator UnloadCoroutine()
    {
        string name = useString ? sceneName : scene.SceneName;
        Debug.Log("started unloading " + name);
        AsyncOperation operation = SceneManager.UnloadSceneAsync(name);

        while (!operation.isDone)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Debug.Log("finished unloading " + name);

        yield return null;
    }
}
