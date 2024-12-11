using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPlay : MonoBehaviour
{
    [SerializeField] private int firstLevel = 1;
    //[SerializeField] private int contunueLevel = -1;
    [SerializeField] GameObject continueButton;
    //[SerializeField] SaveLevelProgress progress;


    private void Start()
    {
        //contunueLevel = progress.Level;
        //continueButton.SetActive(contunueLevel > 0);

        continueButton.SetActive(SaveGameManager.HasSaveFile());

    }

    public void StartNewGame()
    {
        //progress.Level = firstLevel;
        Debug.Log("*************StartNewGame***********");
        SceneGroupLoader.Instance.LoadGroup(firstLevel);
        //SceneManager.LoadScene(4);
    }

    public void ContinueGame()
    {
        //contunueLevel = progress.Level;
        //continueButton.SetActive(contunueLevel > 0);
        SaveGameManager.LoadData();
    }

    private void OnApplicationQuit()
    {
        //progress.Save();
    }
}
