using UnityEngine;
using UnityEngine.UI;

public class StaminaHUD : MonoBehaviour
{
    PlayerDataSO playerData;
    float maximumValue;
    float currentValue;
    float oldValue = 0;
    float timer = 0;
    float fadeOutTimer;
    float fadeInTimer;
    bool idle = false;
    public float MaxTimer = 3.0f;
    public Image progressBar;

    void Start()
    {
        if (!GetPlayerData())
        {
            return;
        }
        progressBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        maximumValue = playerData.Stamina.MaxValue;
        timer = MaxTimer;
    }

    void FixedUpdate()
    {
        if (!playerData) { return; }
        currentValue = playerData.PlayerStamina;
        if (CheckIdle()) { return; }
        CalculateProgress();
        oldValue = currentValue;
    }
    bool GetPlayerData()
    {
        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
        playerData = data.playerData;
        return playerData;
    }
    void CalculateProgress()
    {
        float percentage = currentValue / maximumValue;
        float alpha = 255.0f * percentage;
        Color targetColor = new Color(1.0f, 1.0f, 1.0f, percentage);
        if (idle)
        {
            fadeInTimer += Time.deltaTime * 3;
            if (fadeInTimer >= 1.0f) { idle = false; }
            targetColor = Color.Lerp(progressBar.color, targetColor, fadeInTimer);
        }
        progressBar.color = targetColor;
        progressBar.fillAmount = percentage;
    }
    bool CheckIdle()
    {
        if(currentValue == oldValue && oldValue == maximumValue)
        {
            if (timer < 0)
            {
                fadeOutTimer += Time.deltaTime;
                fadeOutTimer = Mathf.Clamp(fadeOutTimer, 0.0f, 1.0f);
                if(fadeOutTimer >= 1.0f) { idle = true; }
                Color target = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadeOutTimer);
                progressBar.color = target;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            return true;
        }
        fadeOutTimer = 0;
        fadeInTimer = 0;
        timer = MaxTimer;
        return false;
    }
}
