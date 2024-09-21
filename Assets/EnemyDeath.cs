using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    Body body;
    RagdollController controller;
    void Start()
    {
        body = GetComponentInChildren<Body>();
        body.StateChanged += StateChanged;
        controller = GetComponentInChildren<RagdollController>();
    }

    private void StateChanged()
    {
        //Debug.Log("State changed");
        
        if(!body.HasArms || !body.HasLegs)
        { 
            controller.EnableRagdoll();
        }
        else
        {
            controller.DisableRagdoll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
