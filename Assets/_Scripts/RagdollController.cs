
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody[] rbs;
    [SerializeField]private Animator animator;

    [SerializeField] private Behaviour[] disableList;
        
    private Transform hip;
    public Transform Hip => hip;

    //public UnityEvent OnRagDol

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
        if (disableList != null)
        {

            for (int i = 0; i < disableList.Length; i++)
            {
                disableList[i].enabled = true;
            }
        }

    }

    [MakeButton(false)]
    public void EnableRagdoll()
    {

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = false;
        }
         animator.enabled = false;
        if (disableList != null)
        {
            for (int i = 0; i < disableList.Length; i++)
            {
                disableList[i].enabled = false;
            }
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
