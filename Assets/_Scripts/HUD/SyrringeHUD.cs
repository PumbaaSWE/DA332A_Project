using TMPro;
using UnityEngine;

public class SyrringeHUD : TemplateTextHUD
{
    [SerializeField] TMP_Text amount;
    [SerializeField] TMP_Text label;
    int maxAmount;

    protected override void Initialize()
    {
        SyrringeUser temp = playerData.SyrringeUser;
        if (temp)
        {
            maxAmount = temp.MaxSyrringes;
        }
        idleTimerMax = 3.0f;
        defaultColor = Color.white;
        targets = new TMP_Text[2];
        targets[0] = amount;
        targets[1] = label;
    }

    protected override void Handle()
    {
        amount.text = playerData.PlayerSyrringes.ToString() + " / " + maxAmount.ToString();
    }

    protected override bool CheckIdle()
    {
        return playerData.PlayerSyrringes <= 0;
    }

    //void Start()
    //{
    //    if (!playerData)
    //    {
    //        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
    //        playerData = data.playerData;
    //    }
    //    SyrringeUser temp = playerData.SyrringeUser;
    //    if (temp)
    //    {
    //        maxAmount = temp.MaxSyrringes;
    //    }
    //    idleTimer = idleTimerMax;
    //}

    //void Update()
    //{
    //    if (!playerData) { return; }

    //    HandleIdle();
    //}

    //void HandleIdle()
    //{
    //    if (playerData.PlayerSyrringes > 0)
    //    {
    //        amount.color = Color.white;
    //        label.color = Color.white;
    //        idleTimer = idleTimerMax;
    //        return;
    //    }
    //    if (idleTimer <= 0)
    //    {
    //        fadeOutTimer += Time.deltaTime;
    //        fadeOutTimer = Mathf.Clamp(fadeOutTimer, 0.0f, 1.0f);
    //        Color target = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadeOutTimer);
    //        amount.color = target;
    //        label.color = target;
    //        return;
    //    }
    //    fadeOutTimer = 0;
    //    idleTimer -= Time.deltaTime;
    //}
}
