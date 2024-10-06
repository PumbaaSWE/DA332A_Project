using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerDataSO playerData;
    [SerializeField] Transform playerHead;
    public Vector3 LookDir => playerHead.forward;
    public Vector3 HeadPos => playerHead.position;
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
