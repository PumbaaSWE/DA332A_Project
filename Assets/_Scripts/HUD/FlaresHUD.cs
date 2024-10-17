using TMPro;
using UnityEngine;

public class FlaresHUD : MonoBehaviour
{
    PlayerDataSO playerData;
    [SerializeField] TMP_Text amount;
    [SerializeField] TMP_Text maxAmount;
    FlareThrower flareThrower;



    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (!playerData)
        {
            PlayerDataHUDHolder data = GetComponentInParent<PlayerDataHUDHolder>();
            playerData = data.playerData;
            playerData.NotifyOnPlayerChanged(OnPlayer);
        }
    }

    private void OnDisable()
    {
        playerData.UnsubscribeOnPlayerChanged(OnPlayer);
    }

    private void OnPlayer(Transform obj)
    {
        flareThrower = obj.GetComponent<FlareThrower>();
        gameObject.SetActive(flareThrower);
    }

    // Update is called once per frame
    void Update()
    {
        if (flareThrower)
        {
            amount.text = flareThrower.NumFlares.ToString() + " / " + flareThrower.MaxNumFlares.ToString();
        }
    }
}
