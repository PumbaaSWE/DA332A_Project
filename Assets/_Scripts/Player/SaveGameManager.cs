using UnityEngine;

public class SaveGameManager : PersistentSingleton<SaveGameManager>
{
    public Player playerPrefab;
    EventBinding<LoadedEvent> eventBinding;
    IDataService dataService;
    GameData gameData;

    protected override void Awake()
    {
        base.Awake();
        eventBinding = new EventBinding<LoadedEvent>(LoadedEventCallback);
        dataService = new FileDataService(new JsonSerializer(), "json");
    }

    private void OnEnable()
    {
        EventBus<LoadedEvent>.Register(eventBinding);
    }

    private void OnDisable()
    {
        EventBus<LoadedEvent>.Deregister(eventBinding);
    }

    private void LoadedEventCallback(LoadedEvent loadedEvent)
    {
        if (loadedEvent.reloadedSceneGroup)
        {
            Debug.Log("Group reloaded");

            Player p = FindAnyObjectByType<Player>();

            //if (!p)
            //{

            //}
            if(gameData == null)
            {
                gameData = new GameData(); //default values...
            }

            PlayerData playerData = gameData.playerData;
            p.GetComponent<Health>().SetHealth(playerData.health);
            FlareThrower flareThrower = p.GetComponent<Health>().GetComponent<FlareThrower>();
            flareThrower.numFlares = playerData.numFlares;
        }
    }

    private void LoadDataInternal()
    {
        Debug.Log("LoadDataInternal");
        gameData = dataService.Load<GameData>("GameData");
        SceneGroupLoader.Instance.ReloadGroup(gameData.id);
    }

    public static bool HasSaveFile()
    {
        return Instance.dataService.Exists("GameData");
    }

    private void SaveDataInternal()
    {
        Debug.Log("SaveDataInternal");
        GameData gameData = new GameData();
        PlayerData playerData = new();
        gameData.playerData = playerData;
        Player p = FindAnyObjectByType<Player>();
        playerData.health = p.GetComponent<Health>().Value;
        FlareThrower flareThrower = p.GetComponent<Health>().GetComponent<FlareThrower>();
        playerData.numFlares = flareThrower.numFlares;
        gameData.id = SceneGroupLoader.Instance.LastLoaded;
        //WeaponHandler wh = playerDataSO.PlayerTransform.GetComponent<WeaponHandler>();
        //playerData.numShutgunShells = wh.GetAmmoCountFor(Cartridgetype.ShotgunShell);
        dataService.Save("GameData", gameData);
    }

    public static void SaveData()
    {
        Instance.SaveDataInternal();
    }

    public static void LoadData()
    {
        Instance.LoadDataInternal();
    }
}
