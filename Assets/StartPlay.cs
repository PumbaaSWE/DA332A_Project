using UnityEngine;

public class StartPlay : MonoBehaviour
{

    [SerializeField] private int firstLevel = 1;

    public void StartFirstLevel()
    {
        SceneGroupLoader.Instance.LoadGroup(firstLevel);
    }

    public void StartLevel(int index)
    {
        SceneGroupLoader.Instance.LoadGroup(index);
    }
}
