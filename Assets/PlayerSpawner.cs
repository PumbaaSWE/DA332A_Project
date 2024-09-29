using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    Spawner spawner;
    [SerializeField] PlayerDataSO player;

    public Spawner Spawner => spawner;

    // Start is called before the first frame update
    void Start()
    {
        spawner = GetComponent<Spawner>();
        SceneGroupLoader.Instance.OnLoadingComplete += OnLoadingComplete;

        //what if the loading completes before start is called? If unloading is quick af of other scene?
    }

    private void OnLoadingComplete()
    {
        if (!player.PlayerTransform)
        {
            Camera cam = Camera.main;
            if (cam)
                cam.gameObject.SetActive(false);

            spawner.SpawnImmidiate();
        }
    }
}
