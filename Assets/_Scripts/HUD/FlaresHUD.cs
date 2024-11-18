using TMPro;
using UnityEngine;

public class FlaresHUD : TemplateTextHUD
{
    [SerializeField] TMP_Text amount;
    [SerializeField] TMP_Text label;
    FlareThrower flareThrower;

    private void OnEnable()
    {
        if (!playerData)
        {
            PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
            playerData = data.playerData;
            playerData.NotifyOnPlayerChanged(OnPlayer);
        }
    }

    private void OnDisable()
    {
        playerData.UnsubscribeOnPlayerChanged(OnPlayer);
    }

    private void OnPlayer(Transform obj)
    {
        flareThrower = obj.GetComponent<FlareThrower>();
        gameObject.SetActive(flareThrower);
    }

    protected override void Initialize()
    {
        idleTimerMax = 3.0f;
        defaultColor = Color.white;
        if (targets != null) { return; }
        targets = new TMP_Text[2];
        targets[0] = amount;
        targets[1] = label;
    }

    protected override void Handle()
    {
        if (flareThrower)
        {
            amount.text = flareThrower.NumFlares.ToString() + " / " + flareThrower.MaxNumFlares.ToString();
        }
    }

    protected override bool CheckIdle()
    {
        if (flareThrower)
        {
            return flareThrower.NumFlares <= 0;
        }
        return true;
    }
}
