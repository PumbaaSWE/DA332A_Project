using UnityEngine;
[CreateAssetMenu(fileName = "LevelOrder", menuName = "ScriptableObjects/LevelOrder", order = 101)]
public class LevelOrderData : ScriptableObject
{

    //public SceneGroup mainMenu;
    //public SceneGroup loadingScreen;
    //public SceneGroup endingScreen;
    //public SceneGroup deathScreen;


    public int currentLevel = 0;
    public SceneGroup[] levels;
}
