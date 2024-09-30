using TMPro;
using UnityEngine;

public class HealthHud : MonoBehaviour
{
    PlayerDEBUGScript player; // THIS IS A TEMPORARY DEBUG SCRIPT, WHEN ACTUAL PLAYER SCRIPT WITH HP PROPERTY REPLACE THIS SCRIPT WITH THAT
    [SerializeField] TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            player = FindAnyObjectByType<PlayerDEBUGScript/*REPLACE THIS TYPE WITH FINAL PLAYER SCRIPT (READ ABOVE)*/>();
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
