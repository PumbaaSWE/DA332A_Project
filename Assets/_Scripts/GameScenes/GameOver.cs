using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Start()
    {
      
        StartCoroutine(RestartGameAfterDelay());
    }

    private IEnumerator RestartGameAfterDelay()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("EnemyScene");  
    }
}
