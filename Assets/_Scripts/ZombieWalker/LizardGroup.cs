using UnityEngine;

public class LizardGroup : MonoBehaviour
{
    [SerializeField] SimpleAgent[] agents;
    [SerializeField] bool enableOnStart = false;
    [SerializeField] bool getChildren = false;
   
    void Start()
    {
        if (getChildren)
        {
            agents = GetComponentsInChildren<SimpleAgent>(true);
        }
        if (enableOnStart) EnableGroup();
        else
        {
            DisableGroup();
        }


    }
    [MakeButton]
    public void EnableGroup()
    {
        if (agents == null) return;
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].enabled = true;
            agents[i].gameObject.SetActive(true);
            agents[i].WanderTo(transform.position);
        }
    }
    public void DisableGroup()
    {
        if (agents == null) return;
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].enabled = false;
        }
    }
}
