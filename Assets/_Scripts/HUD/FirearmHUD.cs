using TMPro;
using UnityEngine;

public class FirearmHUD : MonoBehaviour
{
    PlayerDataSO playerData;
    WeaponHandler weaponHandler;
    [SerializeField] TMP_Text loadedAmmo;
    [SerializeField] TMP_Text reserveAmmo;


    public void SetFirearm(WeaponHandler weaponHandler)
    {
        this.weaponHandler = weaponHandler;
    }


    void Start()
    {
        if (!playerData)
        {
            PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
            playerData = data.playerData;
        }
        //if (!playerData)
        //{
        //    playerData = FindAnyObjectByType<PlayerDataSO>();
        //}
        if (!weaponHandler && playerData)
        {
            FindEquippedFireArm();
        }
    }
    void FindEquippedFireArm()
    {
        Transform playerParent = playerData.PlayerTransform;
        if (playerParent != null) {
            weaponHandler = playerParent.GetComponent<WeaponHandler>();
            //TooltipUtil.Display("Press left click to shoot", 10.0f);
        }
    }

    // Update is called once per frame
    void Update()
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
}
