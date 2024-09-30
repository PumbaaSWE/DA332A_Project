using TMPro;
using UnityEngine;

public class FirearmHUD : MonoBehaviour
{
    PlayerDataSO playerData;
    Firearm firearm;
    [SerializeField] TMP_Text loadedAmmo;
    [SerializeField] TMP_Text reserveAmmo;


    public void SetFirearm(Firearm firearm)
    {
        this.firearm = firearm;
    }


    void Start()
    {
        if (!playerData)
        {
            playerData = FindAnyObjectByType<PlayerDataSO>();
        }
        if (!firearm && playerData)
        {
            FindEquippedFireArm();
        }
    }
    void FindEquippedFireArm()
    {
        Transform playerParent = playerData.PlayerTransform;
        if (playerParent != null) {
            firearm = playerParent.GetComponentInChildren<Firearm>();
            //TooltipUtil.Display("Press left click to shoot", 10.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (firearm)
        {
            loadedAmmo.text = firearm.LoadedAmmo.ToString();
            reserveAmmo.text = $"/  {firearm.ReserveAmmo.ToString()}";
        }
    }
}
