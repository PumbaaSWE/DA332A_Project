
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] rbs;
    [SerializeField]private Animator animator;

    [SerializeField] private Behaviour[] disableList;
        
    private Transform hip;
    public Transform Hip => hip;

    //public UnityEvent OnRagDol
    private bool active;
    public bool Active { get {return active; } set { if (value == active) return; if (value) EnableRagdoll(); else DisableRagdoll();  }
            }
    // Start is called before the first frame update
    void Awake()
    {
        if(!animator)animator = GetComponentInChildren<Animator>();
        hip = animator.GetBoneTransform(HumanBodyBones.Hips);
        rbs = hip.GetComponentsInChildren<Rigidbody>();
        EnableProjection();
        DisableRagdoll();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [MakeButton(false)]
    public void DisableRagdoll()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }
        animator.enabled = true;
        animator.SetLayerWeight(1, 1);
        //animator.p
        //animator.SetBool("DownedState", false);
        if (disableList != null)
        {

            for (int i = 0; i < disableList.Length; i++)
            {
                disableList[i].enabled = true;
            }
        }
        active = false;

    }

    [MakeButton(false)]
    public void EnableRagdoll()
    {

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = false;
        }
        animator.enabled = false;
        animator.SetLayerWeight(1, 0);
        // animator.SetBool("DownedState", true);
        if (disableList != null)
        {
            for (int i = 0; i < disableList.Length; i++)
            {
                disableList[i].enabled = false;
            }
        }
        active = true;
    }

    public void FreezeRagdoll()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }

    }



    [ContextMenu("Enable Projection..")]
    private void EnableProjection()
    {
        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>(true);
        foreach (var joint in joints)
        {
            joint.enableProjection = true;
        }
    }
}
