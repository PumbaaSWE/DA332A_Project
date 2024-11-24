using TMPro;
using UnityEngine;

public class FirearmHUD : TemplateTextHUD
{
    //PlayerDataSO playerData;
    WeaponHandler weaponHandler;
    [SerializeField] TMP_Text loadedAmmo;
    [SerializeField] TMP_Text reserveAmmo;
    [SerializeField] TMP_Text label;

    protected override void Initialize()
    {
        if (!weaponHandler && playerData)
        {
            FindEquippedFireArm();
        }
        idleTimerMax = 3.0f;
        defaultColor = Color.white;
        if(targets != null) { return; }
        targets = new TMP_Text[3];
        targets[0] = loadedAmmo;
        targets[1] = reserveAmmo;
        targets[2] = label;
    }

    protected override void Handle()
    {
        if (weaponHandler)
        {
            loadedAmmo.text = weaponHandler.GetMagazineCount().ToString();
            reserveAmmo.text = $"/  {weaponHandler.GetAmmoCount().ToString()}";
        }
        else
        {
            FindEquippedFireArm();
        }
    }

    protected override bool CheckIdle()
    {
        if (weaponHandler)
        {
            return weaponHandler.GetMagazineCount() <= 0 && weaponHandler.GetAmmoCount() <= 0;
        }
        return true;
    }

    void FindEquippedFireArm()
    {
        Transform playerParent = playerData.PlayerTransform;
        if (playerParent != null)
        {
            weaponHandler = playerParent.GetComponent<WeaponHandler>();
            //TooltipUtil.Display("Press left click to shoot", 10.0f);
        }
    }

    //public void SetFirearm(WeaponHandler weaponHandler)
    //{
    //    this.weaponHandler = weaponHandler;
    //}

    //void Start()
    //{
    //    if (!playerData)
    //    {
    //        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
    //        playerData = data.playerData;
    //    }
    //    //if (!playerData)
    //    //{
    //    //    playerData = FindAnyObjectByType<PlayerDataSO>();
    //    //}

    //}

}
