using UnityEngine;

public class TestSaveSystem : MonoBehaviour
{
    //public LevelOrderData levelOrderData;

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
}
