using System;
using UnityEngine;

public class SceneGroupLoader : PersistentSingleton<SceneGroupLoader>, IProgress<float>
{

    [SerializeField] private SceneGroup[] sceneGroups;
    [SerializeField] private SceneGroupManager sceneGroupManager;


    protected override void Awake()
    {
        base.Awake();
        if(sceneGroupManager)
            sceneGroupManager = GetComponent<SceneGroupManager>();
        if(sceneGroups == null || sceneGroups.Length <= 0)
        {
            return;
        }
        Debug.Assert(sceneGroups != null && sceneGroups.Length > 0, "SceneGroupLoader - Missing sceneGroups");

        //sceneGroupManager.OnSceneLoading += (s) => Debug.Log("Loading: " + s);
        //sceneGroupManager.OnSceneUnloading += (s) => Debug.Log("Unloading: " + s);
        //sceneGroupManager.OnSceneLoaded += (s) => Debug.Log("Compleded load of: " + s);
        //sceneGroupManager.OnSceneUnloaded+= (s) => Debug.Log("Compleded unload of: " + s);
        //sceneGroupManager.OnSceneGroupLoaded += () => Debug.Log("GroupLoading Completed");

        sceneGroupManager.OnSceneGroupLoaded += LoadedCallback;
        LoadGroup(0);
    }


    private void LoadedCallback()
    {
        Debug.Log("OnLoadingComplete?.Invoke();");
        OnLoadingComplete?.Invoke();
    }

    public event Action OnLoadingComplete;

    public void LoadGroup(int index)
    {
        if(!AssertRange(index, sceneGroups))
        {
            return;
        }
        //Debug.Log("Loading: " + index);
        sceneGroupManager.LoadScenes(sceneGroups[index], this);
    }

    public bool AssertRange<T>(int i, T[] array)
    {
        if(i < 0 || i >= array.Length)
        {
            return false;
        }
        return true;
    }

    public void Report(float value)
    {
        
    }
}
