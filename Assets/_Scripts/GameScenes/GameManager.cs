using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Transform target;
    public PlayerDataSO player;

    private void Update()
    {
    }

    public static void GameOver()
    {
        Instance.StartCoroutine(Instance.HandlePlayerDeath());
    }
    private IEnumerator HandlePlayerDeath()
    {
        SceneManager.LoadScene("GameOverScene");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
