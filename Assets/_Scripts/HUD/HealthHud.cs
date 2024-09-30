using TMPro;
using UnityEngine;

public class HealthHud : MonoBehaviour
{
    PlayerDataSO playerData;
    [SerializeField] TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        if (!playerData)
        {
            PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
            playerData = data.playerData;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerData)
        {
            text.text = playerData.PlayerHealth.ToString();
        }
    }
}
