using System.Collections.Generic;
using UnityEngine;

public class StartPlay : MonoBehaviour
{
    [SerializeField] private int firstLevel = 1;
    [SerializeField] private int contunueLevel = -1;
    [SerializeField] GameObject continueButton;
    [SerializeField] SaveLevelProgress progress;


    private void Start()
    {
        contunueLevel = progress.Level;
        continueButton.SetActive(contunueLevel > 0);
        
    }

    public void StartNewGame()
    {
        progress.Level = firstLevel;
        SceneGroupLoader.Instance.LoadGroup(firstLevel);
    }

    public void ContinueGame()
    {
        contunueLevel = progress.Level;
        continueButton.SetActive(contunueLevel > 0);
        if (contunueLevel > 0)
            SceneGroupLoader.Instance.LoadGroup(contunueLevel);
    }

    private void OnApplicationQuit()
    {
        progress.Save();
    }
}
