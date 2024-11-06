using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
public class LoadHudScene : MonoBehaviour
{
    public SceneField hudScene;
    public bool loadOnStart = true;

    private void Start()
    {
        if (loadOnStart)
        {
            LoadHud();
        }
    }

    [MakeButton]
    public void LoadHud()
    {

        if (Application.isPlaying)
        {
            Scene scene = SceneManager.GetSceneByName(hudScene);
            if (!scene.IsValid())
            {
                SceneManager.LoadSceneAsync(hudScene, LoadSceneMode.Additive);
            }
        }
        else
        {
#if UNITY_EDITOR
            Scene scene = EditorSceneManager.GetSceneByName(hudScene);
            if (!scene.IsValid())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/HUD/HUD.unity", OpenSceneMode.Additive);
            }
#endif  
        }
    }


    [MakeButton]
    public void UnloadHud()
    {
        if (Application.isPlaying)
            SceneManager.UnloadSceneAsync(hudScene);
#if UNITY_EDITOR
        else
            EditorSceneManager.CloseScene(SceneManager.GetSceneByName(hudScene), true);
#endif
    }

}
