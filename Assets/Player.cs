using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]PlayerDataSO playerData;
    void Awake()
    {
        gameObject.tag = "Player";
        playerData.PlayerTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
