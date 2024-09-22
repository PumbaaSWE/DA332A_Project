using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    Body body;
    RagdollController controller;
    //WallClimber wallClimber

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
            Vector3 pos = controller.Hip.position;
            if(Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1, ~0, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point;
                transform.up = hit.normal;
            }
            controller.DisableRagdoll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
