using System;
using UnityEngine;

public class SceneGroupLoader : PersistentSingleton<SceneGroupLoader>
{

    [SerializeField] private SceneGroup[] sceneGroups;
    [SerializeField] private SceneGroupManager sceneGroupManager;
    [SerializeField] private PlayerDataSO playerData;

    public event Action OnLoadingComplete;

    int lastLoaded;


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
        //LoadGroup(0);
    }


    private void LoadedCallback()
    {
        
        OnLoadingComplete?.Invoke();
    }
    [MakeButton(false)]
    public void ReloadGroup(int index)
    {
        LoadGroup(index, true);
    }
    [MakeButton(false)]
    public void ReloadLast()
    {
        ReloadGroup(lastLoaded);
    }
    [MakeButton(false)]
    public void LoadGroup(int index)
    {
        LoadGroup(index, false);
    }

    public void LoadGroup(int index, bool reloadDuplicates)
    {
        if (!AssertRange(index, sceneGroups))
        {
            return;
        }
        playerData.Loading = true;
        lastLoaded = index;
        sceneGroupManager.LoadScenes(sceneGroups[index], reloadDuplicates);
    }

    public bool AssertRange<T>(int i, T[] array)
    {
        if(i < 0 || i >= array.Length)
        {
            return false;
        }
        return true;
    }

}
