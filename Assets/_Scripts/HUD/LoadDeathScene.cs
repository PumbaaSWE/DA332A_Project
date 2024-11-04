using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDeathScene : MonoBehaviour
{
    string death = "Death";
    public SceneField deathScene;

    public void LoadScene()
    {
        if (Application.isPlaying)
        {
            Scene scene = SceneManager.GetSceneByName(death);
            if (!scene.IsValid())
            {
                SceneManager.LoadSceneAsync(deathScene.SceneName, LoadSceneMode.Additive);
            }
        }
    }

    public void UnloadScene()
    {
        if (Application.isPlaying)
            SceneManager.UnloadSceneAsync(death);
    }
}
