using UnityEngine;
using UnityEngine.UI;

public class StaminaHUD : MonoBehaviour
{
    PlayerDataSO playerData;
    float maximumValue;
    float currentValue;
    public Image progressBar;

    void Start()
    {
        if (!GetPlayerData())
        {
            return;
        }
        progressBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        maximumValue = playerData.Stamina.MaxValue;
    }

    void FixedUpdate()
    {
        if (!playerData) { return; }
        currentValue = playerData.PlayerStamina;
        float percentage = currentValue / maximumValue;
        float alpha = 255.0f * percentage;
        progressBar.color = new Color(1.0f, 1.0f, 0.0f, percentage);
        progressBar.fillAmount = percentage;
    }
    bool GetPlayerData()
    {
        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
        playerData = data.playerData;
        return playerData;
    }
}
