using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCreditsScene : MonoBehaviour
{
    [SerializeField] SceneField scene;

    public void LoadCredits()
    {

        if (Application.isPlaying)
        {
            Scene s = SceneManager.GetSceneByName(scene);
            if (!s.IsValid())
            {
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
    }
}
