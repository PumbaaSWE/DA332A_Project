using System;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerDataInfo", menuName = "ScriptableObjects/PlayerDataInfo", order = 2)]
public class PlayerDataSO : ScriptableObject
{
    private event Action<Transform> OnPlayerChanged;

    private Transform playerTransform;

    public Transform PlayerTransform
    {
        get { return playerTransform; }
        set { 
            if(playerTransform != value)
            {
                playerTransform = value;
                OnPlayerChanged?.Invoke(playerTransform);
            }
        }
    }

    public void NotifyOnPlayerChanged(Action<Transform> action)
    {
        OnPlayerChanged += action;
        if (PlayerTransform)
        {
            action.Invoke(PlayerTransform);
        }
    }

    public void UnsubscribeOnPlayerChanged(Action<Transform> action)
    {
        OnPlayerChanged -= action;
    }
}
