using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[DisallowMultipleComponent]
public class SceneGroupManager : MonoBehaviour
{

    public event Action<string> OnSceneLoading = delegate { };
    public event Action<string> OnSceneUnloading = delegate { };
    public event Action<string> OnSceneLoaded = delegate { };
    public event Action<string> OnSceneUnloaded= delegate { };
    public event Action OnSceneGroupLoaded = delegate { };

    private SceneGroup activeSceneGroup;

    public SceneField loadingScene;

   // private readonly WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.1f);
    public string ignoreOnUnload = string.Empty;
    public bool unloadUnusedAssets = true;

    public void LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDuplicates = false)
    {
        StartCoroutine(LoadScenesCouroutine(group, progress, reloadDuplicates));
    }

    public void UnloadScenes()
    {
        StartCoroutine(UnloadScenesCouroutine());
    }

    public IEnumerator LoadScenesCouroutine(SceneGroup group, IProgress<float> progress, bool reloadDuplicates = false)
    {
        activeSceneGroup = group;
        List<string> loadedScenes = new();
        List<string> scenesToUnload = new();
        //List<string> scenesToLoad = new();
        //yield return UnloadScenesCouroutine();

        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            loadedScenes.Add(sceneName);
            if (!group.scenes.Any(sceneData => sceneData.Name == sceneName)) //add condition to not unload some scenes?
            {
                scenesToUnload.Add(sceneName); //the loaded scene does not appear in our group to load
            }
        }
        bool loadingSceneUp = false;
        if(scenesToUnload.Count == sceneCount)
        {
            //this is not allowed! Load
            SceneManager.LoadScene(loadingScene);
            loadingSceneUp = true;
        }

        if(!loadingSceneUp && scenesToUnload.Count > 0)
        {
            AsyncOperationsGroup unloadGroup = new(scenesToUnload.Count);
            foreach (var scene in scenesToUnload)
            {
                string s = scene;
                //Debug.Log("Trying to unload " + s);
                if (scene == null) continue;
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                OnSceneUnloading.Invoke(s);
                operation.completed += obj => { OnSceneUnloaded.Invoke(s); };
                unloadGroup.Add(operation);
            }
            //while (!unloadGroup.IsDone)
            //{
            //    yield return new WaitForSecondsRealtime(0.1f);
            //}
            yield return new WaitUntil(() => unloadGroup.IsDone); //what if we dont wait before loading? faster? chaos?
        }

        int numScenesToLoad = activeSceneGroup.Count;
        AsyncOperationsGroup operationsGroup = new(numScenesToLoad);


        for (int i = 0; i < numScenesToLoad; i++)
        {
            SceneData sceneData = group.scenes[i];
            if(!reloadDuplicates && loadedScenes.Contains(sceneData.Name))
            {
                continue;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.scene, LoadSceneMode.Additive);

            operation.completed += obj => { OnSceneLoaded.Invoke(sceneData.Name); };

            operationsGroup.Add(operation);
            OnSceneLoading.Invoke(sceneData.Name); //this is started loading, not finished
        }

        while (!operationsGroup.IsDone)
        {
            
            progress?.Report(operationsGroup.Progress);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Scene activeScene = SceneManager.GetSceneByName(activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

        if (loadingSceneUp)
        {
            SceneManager.UnloadSceneAsync(loadingScene);
        }

        if (activeScene.IsValid())
        {
            SceneManager.SetActiveScene(activeScene);
        }

        

        OnSceneGroupLoaded.Invoke();
    }

    public IEnumerator UnloadScenesCouroutine()
    {
        List<string> scenesToUnload = new();
        string activeScene = SceneManager.GetActiveScene().name;
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene sceneAt = SceneManager.GetSceneAt(i);
            if (!sceneAt.isLoaded) continue;
            string sceneName = sceneAt.name;
            if (sceneName == activeScene || sceneName == ignoreOnUnload)
            {
                continue;
            }
            
            scenesToUnload.Add(sceneName);
        }
        AsyncOperationsGroup operationsGroup = new(scenesToUnload.Count);

        foreach (var scene in scenesToUnload)
        {
            AsyncOperation opereration = SceneManager.UnloadSceneAsync(scene);
            if(opereration == null) continue;

            operationsGroup.Add(opereration);

            OnSceneUnloading.Invoke(scene);
        }

        while (!operationsGroup.IsDone)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        if (unloadUnusedAssets)
        {
            AsyncOperation operation = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => operation.isDone);
        }
    }
}

public struct AsyncOperationsGroup
{
    public readonly List<AsyncOperation> operations;
    public float Progress => operations.Count == 0 ? 0 : operations.Average(o=>o.progress);
    public bool IsDone => operations.All(o=>o.isDone);

    public AsyncOperationsGroup(int capacity)
    {
        operations = new List<AsyncOperation>(capacity);
    }

    public void Add(AsyncOperation operation) => operations.Add(operation);
}
