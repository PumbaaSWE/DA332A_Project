using TMPro;
using UnityEngine;

public class HealthHud : MonoBehaviour
{
    PlayerTempScript player;
    [SerializeField] TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            player = FindAnyObjectByType<PlayerTempScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            text.text = player.HP.ToString();
        }
    }
}
