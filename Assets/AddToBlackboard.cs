using UnityEngine;

public class AddToBlackboard : MonoBehaviour
{
    
    public string toAdd;
    
    public void Add(bool b)
    {
        Blackboard.Instance.Set(toAdd, b);
    }

    public void AddTrue()
    {
        Blackboard.Instance.Set(toAdd, true);
    }

}
