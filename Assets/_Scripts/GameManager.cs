using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager :  Singleton<DetectableTargetManager>
{
    [SerializeField] Transform target;
    public PlayerDataSO player;


    private void Update()
    {
        if(player.health.dead == true)
        {
            StartCoroutine(HandlePlayerDeath());
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        SceneManager.LoadScene("GameOverScene");

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
