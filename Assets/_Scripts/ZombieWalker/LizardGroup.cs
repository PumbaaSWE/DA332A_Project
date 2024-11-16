using UnityEngine;

public class LizardGroup : MonoBehaviour
{
    [SerializeField] SimpleAgent[] agents;
    [SerializeField] bool enableOnStart = false;
   
    void Start()
    {
        if (enableOnStart) EnableGroup();
        else
        {
            DisableGroup();
        }
    }

    public void EnableGroup()
    {
        if (agents == null) return;
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].enabled = true;
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
