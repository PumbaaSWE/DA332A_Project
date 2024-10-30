using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
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
        else
        {
#if UNITY_EDITOR
            Scene scene = EditorSceneManager.GetSceneByName(death);
            if (!scene.IsValid())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/HUD/HUD.unity", OpenSceneMode.Additive);
            }
#endif  
        }
    }

    public void UnloadScene()
    {
        if (Application.isPlaying)
            SceneManager.UnloadSceneAsync(death);
#if UNITY_EDITOR
        else
            EditorSceneManager.CloseScene(SceneManager.GetSceneByName(deathScene.SceneName), true);
#endif
    }
}
