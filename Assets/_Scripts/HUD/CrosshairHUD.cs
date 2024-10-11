using UnityEngine;
using UnityEngine.UI;

public class CrosshairHUD : MonoBehaviour
{
    [SerializeField] RectTransform right;
    [SerializeField] RectTransform left;
    [SerializeField] RectTransform top;
    [SerializeField] RectTransform bottom;
    PlayerDataSO playerData;
    WeaponHandler weaponHandler;
    [SerializeField] private float degreesIncrease = 1.0f;
    [SerializeField] private float degreesDecay = 7.0f;
    private float degrees = 0;
    private float minDegrees = 0;
    private float maxDegrees = 10.0f;
    private int oldMagazineCount = 0;
    private float offset = 20;
    private float maxOffset = 20;
    private float minOffset = 15;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerData)
        {
            PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
            playerData = data.playerData;
        }
        if(!weaponHandler && playerData)
        {
            FindEquippedFireArm();
        }
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

    // Update is called once per frame
    void Update()
    {

        if (weaponHandler)
        {
            CalculateDegrees();        
        }
        degrees = Mathf.Clamp(degrees, minDegrees, maxDegrees);
        Camera cam = Camera.main;
        float height = cam.pixelHeight;
        float d = degrees / cam.fieldOfView;
        float p = height * d + offset;
        top.transform.localPosition = new Vector3(0, p, 0);
        left.transform.localPosition = new Vector3(-p, 0, 0);
        right.transform.localPosition = new Vector3(p, 0, 0);
        bottom.transform.localPosition = new Vector3(0, -p, 0);
    }
    void CalculateDegrees()
    {
        if (weaponHandler.EquippedGun)
        {
            if (weaponHandler.EquippedGun.Firing && degrees <= (maxDegrees - (degreesIncrease/2)))
            {
                
                int delta = oldMagazineCount - weaponHandler.GetMagazineCount();
                delta = Mathf.Clamp(delta, 0, oldMagazineCount);
                if (delta > 0)
                {
                    degrees = weaponHandler.EquippedGun.HipFireSpread;
                }
                //degrees += degreesIncrease * delta;
            }
            else
            {
                degrees -= degreesDecay * Time.deltaTime;
            }
            oldMagazineCount = weaponHandler.GetMagazineCount();
            if (weaponHandler.EquippedGun.Ads)
            {
                offset -= degreesDecay * Time.deltaTime;
                offset = Mathf.Clamp(offset, minOffset, maxOffset);
            }
            else
            {
                offset += degreesDecay * Time.deltaTime * 2;
                offset = Mathf.Clamp(offset, minOffset, maxOffset);
            }
        }
    }
}
