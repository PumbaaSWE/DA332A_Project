using UnityEngine;

public class TestSaveSystem : MonoBehaviour
{
    //public LevelOrderData levelOrderData;

    bool enabledLoading = false;

    [MakeButton(false)]
    public void LoadLevel(int i)
    {
        SceneGroupLoader.Instance.LoadGroup(i);

    }

    [MakeButton(false)]
    public void ReloadLevel(int i)
    {
        SceneGroupLoader.Instance.ReloadGroup(i);

    }

    [MakeButton(false)]
    public void Save()
    {
        SaveGameManager.SaveData();
    }

    [MakeButton(false)]
    public void Load()
    {
        if (!SaveGameManager.HasSaveFile())
        {
            Debug.LogWarning("No save file!");
            return;
        }
        SaveGameManager.LoadData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            enabledLoading = !enabledLoading;
            TooltipUtil.Display(enabledLoading ? "Loading maps enabled" : "Loading maps disabled", 1f);
        }
        if (enabledLoading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                LoadLevel(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                LoadLevel(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                LoadLevel(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                LoadLevel(4);
            }
        }
    }
}
