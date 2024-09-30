using TMPro;
using UnityEngine;

public class HealthHud : MonoBehaviour
{
    PlayerDataSO playerData;
    [SerializeField] TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            playerData = FindAnyObjectByType<PlayerDataSO>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            text.text = playerData.PlayerHealth.ToString();
        }
    }
}
