using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthHud : MonoBehaviour
{
    PlayerDataSO playerData;
    [SerializeField] TMP_Text text;
    [SerializeField] Image progressBar;
    float maxHealth;
    [SerializeField] float currentHealth;
    [Tooltip("offset float between [0-1]")]
    [SerializeField] float offset = 0;
    public bool debug;
    // Start is called before the first frame update
    void Start()
    {
        if (!GetPlayerData())
        {
            return;
        }
        progressBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        maxHealth = playerData.Health.MaxHealth;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerData)
        {
            return;
            //text.text = playerData.PlayerHealth.ToString();
        }
        if (!debug)
        {
            currentHealth = playerData.PlayerHealth;
        }
        float percentage = currentHealth / maxHealth;
        progressBar.fillAmount = percentage; // apply progress first, calculate color after
        percentage -= 0.5f * (1 - (percentage * 2 - 1)); // when at half health, percentage = 0. When at full health, percentage = 1.
        percentage = Mathf.Clamp(percentage, 0.0f, 1.0f);
        float R = 1.0f;
        float G = 1.0f;
        if (percentage >= 0.5f)
        {
            R = 1 * (1 - (percentage * 2 - 1));
        }
        if(percentage <= 0.5f)
        {
            G = 2 * percentage;
        }
        R -= offset;
        G -= offset;
        R = Mathf.Clamp(R, 0.0f, 1.0f);
        G = Mathf.Clamp(G, 0.0f, 1.0f);
        progressBar.color = new Color(R, G, 0.0f, 1.0f);
    }
    bool GetPlayerData()
    {
        PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
        playerData = data.playerData;
        return playerData;
    }
}
