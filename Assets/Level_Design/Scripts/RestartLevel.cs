using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    [SerializeField] string levelName;
    public void RestartRightLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
