using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class TemplateTextHUD : MonoBehaviour
{
    // Start is called before the first frame update
    protected PlayerDataSO playerData;
    [SerializeField] protected TMP_Text[] targets;
    float idleTimer;
    float idleTimerMin;
    protected float idleTimerMax;
    float fadeOutTimer;
    protected Color defaultColor;
    Color color;
    void Start()
    {
        idleTimerMin = 0.0f;
        if (!GetPlayerData()) { return; }
        Initialize();
        idleTimer = idleTimerMax = idleTimerMax > 0.0f ? idleTimerMax : 3.0f;
        color = defaultColor = defaultColor != null ? defaultColor : Color.white;
    }
    bool GetPlayerData()
    {
        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
        playerData = data.playerData;
        return playerData;
    }
    //void OnEnable()
    //{
    //    if (!playerData)
    //    {
    //        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
    //        if (!data) { return; }
    //        playerData = data.playerData;
    //        playerData.NotifyOnPlayerChanged(OnPlayer);
    //    }
    //}
    //void OnDisable()
    //{
    //    playerData.UnsubscribeOnPlayerChanged(OnPlayer);
    //}
    //private void OnPlayer(Transform obj)
    //{
    //    flareThrower = obj.GetComponent<FlareThrower>();
    //    gameObject.SetActive(flareThrower);
    //}
    // Update is called once per frame
    void Update()
    {
        if (!playerData) { return; }
        Handle();
        if (CheckIdle())
        {
            HandleIdle();
        }
        else
        {
            color = defaultColor;
            idleTimer = idleTimerMax;
        }
        foreach(TMP_Text target in targets)
        {
            target.color = color;
        }
    }

    void HandleIdle()
    {
        if (idleTimer <= 0)
        {
            fadeOutTimer += Time.deltaTime;
            fadeOutTimer = Mathf.Clamp(fadeOutTimer, 0.0f, 1.0f);
            color = Color.Lerp(defaultColor, new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.0f), fadeOutTimer);
            return;
        }
        fadeOutTimer = 0;
        idleTimer -= Time.deltaTime;
    }

    protected abstract void Initialize(); // set values for colors and timers
    protected abstract void Handle(); // apply all the updates values from the player
    protected abstract bool CheckIdle(); // define idle logics
}
