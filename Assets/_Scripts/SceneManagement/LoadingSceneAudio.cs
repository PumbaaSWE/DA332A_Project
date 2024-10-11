using UnityEngine;

public class LoadingSceneAudio : MonoBehaviour
{
    [SerializeField] PlayerDataSO playerData;
    private void Awake()
    {
        playerData.NotifyOnPlayerChanged(OnPlayer);
    }
    private void OnDisable()
    {
        playerData.UnsubscribeOnPlayerChanged(OnPlayer);
    }

    private void OnPlayer(Transform obj)
    {
        GetComponent<AudioListener>().enabled = false;
    }
}
