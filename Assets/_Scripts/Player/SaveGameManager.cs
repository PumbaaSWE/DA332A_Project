using UnityEngine;

public class SaveGameManager : PersistentSingleton<SaveGameManager>
{
    public Player playerPrefab;
    public Firearm[] firearmPrefabs;
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

            WeaponHandler weaponHandler = p.GetComponent<WeaponHandler>();

            // Set ammo
            weaponHandler.AmmunitionPool[Cartridgetype.Rifle] = playerData.numRifleRounds;
            weaponHandler.AmmunitionPool[Cartridgetype.Pistol] = playerData.numPistolRounds;
            weaponHandler.AmmunitionPool[Cartridgetype.ShotgunShell] = playerData.numShotgunShells;

            // Clear weapons
            while (weaponHandler.Guns.Count > 0)
            {
                Destroy(weaponHandler.Guns[0].gameObject);
                weaponHandler.Guns.RemoveAt(0);
            }

            // Set weapons
            if (playerData.weaponData == null) return;

            foreach (var wData in playerData.weaponData)
                foreach(var fPrefab in firearmPrefabs)
                    if (wData.id == fPrefab.Id)
                        weaponHandler.PickupGun(fPrefab.gameObject);
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

        WeaponHandler weaponHandler = p.GetComponent<WeaponHandler>();

        // Get ammo
        playerData.numRifleRounds = weaponHandler.AmmunitionPool[Cartridgetype.Rifle];
        playerData.numPistolRounds = weaponHandler.AmmunitionPool[Cartridgetype.Pistol];
        playerData.numShotgunShells = weaponHandler.AmmunitionPool[Cartridgetype.ShotgunShell];

        // Get weapons
        Firearm[] weaps = weaponHandler.Guns.ToArray();
        WeaponData[] weaponData = new WeaponData[weaps.Length];
        for (int i = 0; i < weaps.Length; i++)
            weaponData[i] = new WeaponData() { id = weaps[i].Id, ammo = weaps[i].LoadedAmmo };
        playerData.weaponData = weaponData;

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
