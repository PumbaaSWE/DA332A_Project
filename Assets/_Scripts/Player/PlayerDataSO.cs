using System;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerDataInfo", menuName = "ScriptableObjects/PlayerDataInfo", order = 2)]
public class PlayerDataSO : ScriptableObject
{
    private event Action<Transform> OnPlayerChanged;

    private Transform playerTransform;

  //  public bool Loading;

    public float PlayerHealth => health ? health.Value : 0;
    public Health health;
    public Health Health => health;

    public float PlayerStamina => stamina ? stamina.Value : 0;
    public Stamina stamina;
    public Stamina Stamina => stamina;

    public int PlayerSyrringes => syrringeUser ? syrringeUser.NumSyrringes : 0;
    private SyrringeUser syrringeUser;
    public SyrringeUser SyrringeUser => syrringeUser;

    public Transform PlayerTransform
    {
        get { return playerTransform; }
        set
        {
            if(playerTransform != value)
            {
                playerTransform = value;
                OnPlayerChanged?.Invoke(playerTransform);
                health = playerTransform.GetComponent<Health>();
                stamina = playerTransform.GetComponent<Stamina>();
                syrringeUser = playerTransform.GetComponent<SyrringeUser>();
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

    private void OnEnable()
    {
        //SceneGroupLoader.Instance.OnLoadingComplete += Instance_OnLoadingComplete;
    }

    private void Instance_OnLoadingComplete()
    {
       // Loading = false;
    }
}
