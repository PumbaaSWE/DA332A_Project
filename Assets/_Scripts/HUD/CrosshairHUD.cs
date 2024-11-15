using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairHUD : MonoBehaviour
{
    [SerializeField] RectTransform right;
    [SerializeField] RectTransform left;
    [SerializeField] RectTransform top;
    [SerializeField] RectTransform bottom;
    RectTransform[] transforms;
    PlayerDataSO playerData;
    WeaponHandler weaponHandler;
    [SerializeField] private float degreesMultiplier = 7.0f;
    private float degrees = 0;
    private float minDegrees = 0;
    private float maxDegrees = 10.0f;
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
        transforms = new RectTransform[4];
        transforms[0] = right;
        transforms[1] = left;
        transforms[2] = top;
        transforms[3] = bottom;
    }
    void FindEquippedFireArm()
    {
        Transform playerParent = playerData.PlayerTransform;
        if (playerParent != null)
        {
            weaponHandler = playerParent.GetComponent<WeaponHandler>();
        }
    }
    void FixedUpdate()
    {

        if (weaponHandler)
        {
            CalculateDegrees();        
        }
        degrees = Mathf.Clamp(degrees, minDegrees, maxDegrees);

        Camera cam = Camera.main;
        if (cam)
        {
            float height = cam.pixelHeight;
            float d = degrees / cam.fieldOfView;
            float p = height * d + offset;
            top.transform.localPosition = new Vector3(0, p, 0);
            left.transform.localPosition = new Vector3(-p, 0, 0);
            right.transform.localPosition = new Vector3(p, 0, 0);
            bottom.transform.localPosition = new Vector3(0, -p, 0);
        }
    }
    void CalculateDegrees()
    {
        degrees = weaponHandler.GetHipfireAngle();
        ApplyAds();
    }
    void ApplyAds()
    {
        float percentage = weaponHandler.GetAdsProcentage();
        offset = minOffset * percentage + (maxOffset * (1 - percentage));
        offset = Mathf.Clamp(offset, minOffset, maxOffset);
        percentage = 0 + (1 - 1 * percentage);
        foreach(RectTransform transform in transforms)
        {
            Image image = transform.GetComponent<Image>();
            Outline outline = transform.GetComponent<Outline>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, percentage);
            outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, percentage);
        }
    }
}
