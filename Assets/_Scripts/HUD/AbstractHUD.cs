using UnityEngine;

public abstract class AbstractHUD<T> : MonoBehaviour where T : Component
{

    [SerializeField] protected PlayerDataSO playerData;
    protected Transform playerTransform;

    protected void Awake()
    {
        playerData = GetPlayerData();
        if (playerData)
        {
            playerData.NotifyOnPlayerChanged(SetPlayer);
        }
    }

    protected virtual void SetPlayer(Transform obj)
    {
        playerTransform = obj;

        if (playerTransform)
        {
            if(playerTransform.TryGetComponent<T>(out var script)) SetScript(script);
        }
        else
        {
            PlayerRemoved();
        }
    }

    /// <summary>
    /// Will call get component with the T type on playerTransform. If the thing is located in children or non-existen this will give you null
    /// </summary>
    /// <param name="script"></param>
    protected abstract void SetScript(T script);

    /// <summary>
    /// Will be called if no player is found, good idea to disable evrything maybe
    /// </summary>
    protected abstract void PlayerRemoved();

    // Update is called once per frame
    protected void OnDestroy()
    {
        if (playerData)
        {
            playerData.UnsubscribeOnPlayerChanged(SetPlayer);
        }
    }

    protected PlayerDataSO GetPlayerData()
    {
        if(playerData) return playerData;
        PlayerDataHUDHolder holder = GetComponentInParent<PlayerDataHUDHolder>();
        if (holder)
        {
            return holder.playerData;
        }
        return null;
    }
}
