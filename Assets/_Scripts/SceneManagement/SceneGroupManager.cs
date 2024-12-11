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

    bool loading;

    public void LoadScenes(SceneGroup group, bool reloadDuplicates = false)
    {
        //Debug.Log("*************LoadScenes***********");
        if (loading)
        {
            Debug.LogError("SceneGroupManager - LoadScenes called before loading was done, please find the reason!");
            return;
        }
        StartCoroutine(LoadScenesCouroutine(group, reloadDuplicates));
    }

    public void UnloadScenes()
    {
        StartCoroutine(UnloadScenesCouroutine());
    }

    public IEnumerator LoadScenesCouroutine(SceneGroup group, bool reloadDuplicates = false)
    {
        //Debug.Log("*************LoadScenesCouroutine***********");
        loading = true;
        Spawner.blockedBecauseLoading = true;
        activeSceneGroup = group;
        List<string> loadedScenes = new();
        List<string> scenesToUnload = new();

        int sceneCount = SceneManager.sceneCount;

        int numScenesToLoad = activeSceneGroup.Count;
        //StringBuilder stringBuilder = new();
        //stringBuilder.Append("\nactiveSceneGroup contains "+ numScenesToLoad + " to load:\n");
        //for (int i = 0; i < numScenesToLoad; i++)
        //{
        //    stringBuilder.AppendLine(activeSceneGroup.scenes[i].Name);
        //}
        //Debug.Log(stringBuilder.ToString());

        if (reloadDuplicates) //we should unload all scenes all scenes
        {           
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                string sceneName = scene.name;
                //Debug.Log("reloadDuplicates - to unload = " + sceneName);

                scenesToUnload.Add(sceneName);     
                
            }
        }
        else
        {
            for (int i = 0; i < sceneCount; i++)
            {
                string sceneName = SceneManager.GetSceneAt(i).name;
                //SceneManager.GetSceneAt(i).isLoaded
                if (!activeSceneGroup.scenes.Any(sceneData => sceneData.Name == sceneName)) //
                {
                    scenesToUnload.Add(sceneName); //the loaded scene does NOT appear in our group to load, mark it to unload
                    //Debug.Log("to unload = " + sceneName);
                }
                else
                {
                    loadedScenes.Add(sceneName); //the loaded scene does appear in our group to load, do not unload
                    //Debug.Log("to keep loaded = " + sceneName);
                }
            }
        }


       // Debug.Log("***************See if we need loading scene***************");
        //Debug.Log(scenesToUnload.Count + "==" + sceneCount);
        bool loadingSceneUp = false;
        if(scenesToUnload.Count == sceneCount) //we are trying to unload all scenes
        {
            //this is not allowed! Load the loading scene
            SceneManager.LoadScene(loadingScene);
            loadingSceneUp = true;
        }
        //We do not have the loading screen up and we have scenes to unload
        if (!loadingSceneUp && scenesToUnload.Count > 0)
        {
            //Debug.Log("***************Do unloading***************");
            AsyncOperationsGroup unloadGroup = new(scenesToUnload.Count);
            foreach (var scene in scenesToUnload)
            {
                string s = scene;
                //Debug.Log("Trying to unload " + s);
                if (scene == null) continue;
                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if(operation == null)
                {
                    Debug.Log("sceneCount:" + sceneCount);
                    PrintList("Loaded Scenes", loadedScenes);
                    PrintList("Scenes To Unload Scenes", scenesToUnload);
                    continue;
                }
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

        //all unloading complete, do the loading
        //int numScenesToLoad = activeSceneGroup.Count;
        AsyncOperationsGroup operationsGroup = new(numScenesToLoad);

        for (int i = 0; i < numScenesToLoad; i++)
        {
            SceneData sceneData = group.scenes[i];
            if(loadedScenes.Contains(sceneData.Name))
            {
                continue;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.scene, LoadSceneMode.Additive);
            operation.allowSceneActivation = !loadingSceneUp;
            operation.completed += obj => { OnSceneLoaded.Invoke(sceneData.Name); };

            operationsGroup.Add(operation);
            OnSceneLoading.Invoke(sceneData.Name); //this is started loading, not finished
        }

        while (!operationsGroup.IsDone)
        {   
            if(operationsGroup.Progress >= .9f && loadingSceneUp)
            {
                operationsGroup.operations.ForEach(op => op.allowSceneActivation = true);
                ActiveAudioListener.enabled = false;
            }

            yield return null;
        }

        if (loadingSceneUp)
        {
            //Debug.Log("*********LoadScenesCouroutine Unload loading Scene***********");
           
            
            //Debug.Log("Loading - Disabeling audio");
        }

        //Debug.Log("*********LoadScenesCouroutine Set Active***********");
        Scene activeScene = SceneManager.GetSceneByName(activeSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
        if (activeScene.IsValid())
        {
            yield return new WaitUntil(() => activeScene.isLoaded);
            yield return null; // Unity thinks you need to wait one frame...
            SceneManager.SetActiveScene(activeScene);
        }



        if (loadingSceneUp)
        {
            //Debug.Log("*********LoadScenesCouroutine Unload loading Scene***********");
            AsyncOperation ao = SceneManager.UnloadSceneAsync(loadingScene);
            ao.completed += LoadingScene_completed;
            yield return new WaitUntil(() => ao.isDone);
        }

        Spawner.blockedBecauseLoading = false;
        loading = false;
        OnSceneGroupLoaded.Invoke();
        //Debug.Log("**********LoadScenesCouroutine ENDS***********");
    }

    private void LoadingScene_completed(AsyncOperation obj)
    {
        
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

    public static void PrintList(string text, List<string> strings)
    {
        Debug.Log(text);
        for (int i = 0; i < strings.Count; i++)
        {
            Debug.Log("1. " + strings[i]);

        }
    }


    private static AudioListener _activeAudioListener;
    public static AudioListener ActiveAudioListener
    {
        get
        {
            if (!_activeAudioListener
                || !_activeAudioListener.isActiveAndEnabled)
            {
                var audioListeners = FindObjectsOfType<AudioListener>(false);
                _activeAudioListener = Array.Find(audioListeners, audioListener => audioListener.enabled); // No need to check isActiveAndEnabled, FindObjectsOfType already filters out inactive objects.
            }

            return _activeAudioListener;
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
