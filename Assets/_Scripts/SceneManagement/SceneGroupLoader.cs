using System;
using UnityEngine;

public class SceneGroupLoader : PersistentSingleton<SceneGroupLoader>
{

    [SerializeField] private SceneGroup[] sceneGroups;
    [SerializeField] private SceneGroupManager sceneGroupManager;
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Player player;

    public event Action OnLoadingComplete;

    int lastLoaded;
    bool groupReloaded;

    public int LastLoaded => lastLoaded;


    protected override void Awake()
    {
        base.Awake();
        if(sceneGroupManager)
            sceneGroupManager = GetComponent<SceneGroupManager>();
        Debug.Assert(sceneGroups != null && sceneGroups.Length > 0, gameObject.name + " - SceneGroupLoader - Missing sceneGroups");

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

        player = FindAnyObjectByType<Player>();

        if (!player)
        {
            Debug.Log("No player founmd");
            PlayerSpawn spawn = FindAnyObjectByType<PlayerSpawn>();
            if (spawn)
            {
                player = Instantiate(playerPrefab, spawn.Position, spawn.Rotation);
            }
            else
            {
                player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            }
        }
        player.EnableAudio();




        EventBus<LoadedEvent>.Raise(new LoadedEvent
        {
            reloadedSceneGroup = groupReloaded,
            sceneGroupIndex = lastLoaded
        });
        groupReloaded = false;
        OnLoadingComplete?.Invoke();
    }
    //[MakeButton(false)]
    public void ReloadGroup(int index)
    {
        LoadGroup(index, true);
        groupReloaded = true;
    }
    //[MakeButton(false)]
    public void ReloadLast()
    {
        ReloadGroup(lastLoaded);
    }
    //[MakeButton(false)]
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
       // playerData.Loading = true;
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


public struct LoadedEvent : IEvent
{
    public bool reloadedSceneGroup;
    public int sceneGroupIndex;
}