
using System.Linq;
using UnityEngine;


public class Ragdoll : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; }
    }

    public enum RagdollState
    {
        Default,
        Ragdoll,
        AlignHip,
        ResettingBones,
        StartCrawl,
    };

    public RagdollState state = RagdollState.Default;
    [SerializeField] private float timeToGetUp = 5;
    [SerializeField] private float resetBonesTime = 1;

    [SerializeField] private string faceUpStateName = "Rolling";
    [SerializeField] private string faceUpClipName = "Rolling";
    [SerializeField] private string faceDownStateName = "Getting Up";
    [SerializeField] private string faceDownClipName = "Getting Up";

    private bool isFacingUp;

    private Transform[] bones;
    private BoneTransform[] ragdollBones;

    private BoneTransform[] faceDownBones;
    private BoneTransform[] faceUpBones;

    private Animator animator;

    private Transform hip;
    private Rigidbody[] rbs;
    private CharacterController controller;

    private float getUpTimer;
    private float resetTimer;
    Regrow regrow;
    FSM_Walker fsm_Walker;
    Limbstate limbstate;

    private void Awake()
    {
        limbstate = GetComponent<Limbstate>();
        fsm_Walker = GetComponent<FSM_Walker>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        hip = animator.GetBoneTransform(HumanBodyBones.Hips);

        rbs = hip.GetComponentsInChildren<Rigidbody>();
        bones = hip.GetComponentsInChildren<Transform>();
        ragdollBones = new BoneTransform[bones.Length];
        faceUpBones = new BoneTransform[bones.Length];
        faceDownBones = new BoneTransform[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            ragdollBones[i] = new BoneTransform();
            faceUpBones[i] = new BoneTransform();
            faceDownBones[i] = new BoneTransform();


        }

        PopulateAnimationBoneTransforms(faceUpClipName, faceUpBones);
        PopulateAnimationBoneTransforms(faceDownClipName, faceDownBones);

        DisableRagdoll();

        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint joint in joints)
        {
            joint.enableProjection = true;
        }

        regrow = GetComponent<Regrow>();
    }
    //private void Start()
    //{
    //    DisableRagdoll();

    //}
    void Update()
    {
        //VisonUpdate();



        switch (state)
        {
            case RagdollState.Default:
                DefaultBehaviour();
                break;
            case RagdollState.Ragdoll:
                RagdollBehaviour();
                break;
            case RagdollState.AlignHip:
                AlignHipBehaviour();
                break;
            case RagdollState.ResettingBones:
                ResettingBonesBehaviour();
                break;
            case RagdollState.StartCrawl:
                CrawlUpBehaviour();
                break;

            default:
                break;
        }
    }


    public void TriggerRagdoll(Vector3 force, Vector3 point)
    {
        
        EnableRagdoll();
        Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        rb.AddForceAtPosition(force, point, ForceMode.Impulse);

        state = RagdollState.Ragdoll;
        getUpTimer = timeToGetUp;
    }

    private void RagdollBehaviour()
    {
        getUpTimer -= Time.deltaTime;
        if (getUpTimer < 0)
        {
            isFacingUp = hip.forward.y > 0;

            state = RagdollState.AlignHip;
            resetTimer = 0;
        }
    }
    private void AlignHipBehaviour()
    {
       
        AlignRotationToHip();
        AlignPositionToHip();
        PopulateBoneTransforms(ragdollBones);
        state = RagdollState.ResettingBones;
        resetTimer = 0;

        return;

    }
    private void ResettingBonesBehaviour()
    {
        resetTimer += Time.deltaTime;
        float t = resetTimer / resetBonesTime;
        int length = bones.Length;
        BoneTransform[] standUpBones = GetStandUpBoneTransforms();

        for (int i = 0; i < length; i++)
        {
            Vector3 pos = Vector3.Lerp(ragdollBones[i].Pos, standUpBones[i].Pos, t);

            Quaternion rot = Quaternion.Lerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, t);
            bones[i].SetLocalPositionAndRotation(pos, rot);
        }

        if (resetTimer >= resetBonesTime)
        {
            state = RagdollState.StartCrawl;
            DisableRagdoll();
            Transition();
            fsm_Walker.agentState = FSM_Walker.AgentState.Idle;
            //animator.Play(StateName(), 0, 0);
        }
    }


    void Transition()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Getting Up") && !stateInfo.IsName("Rolling"))
        {
            if (!isFacingUp)
            {
                animator.Play("Getting Up", 0, 0);
            }
            else
            {
                animator.Play("Rolling", 0, 0);
            }
        }
       
    }

    private void CrawlUpBehaviour()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
        {
            //if(isFacingUp)
            //{
            //    animator.Play("Getting Up", 0, 0);
              
            //}
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up"))
            {
               
               state = RagdollState.Default;
            }

            if (limbstate.limbStatehit == Limbstate.AgentHit.Armless || limbstate.limbStatehit == Limbstate.AgentHit.LegAndArmLess)
            {
                TriggerRagdoll(Vector3.zero, Vector3.zero);
            }
        }
    }
    private void DisableRagdoll()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }
        animator.enabled = true;
        controller.enabled = true;
        //fsm_Walker.agentState = FSM_Walker.AgentState.Wander;
    }

    public void EnableRagdoll()
    {
        fsm_Walker.agentState = FSM_Walker.AgentState.Sleep;
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = false;
        }
        animator.enabled = false;
        controller.enabled = false;
    }

    private void AlignRotationToHip()
    {
        Vector3 originalPos = hip.position;
        Quaternion originalRot = hip.rotation;
        Vector3 desiredDir = hip.up * (isFacingUp ? 1 : 1);
        desiredDir.y = 0;

        Quaternion rot = Quaternion.FromToRotation(transform.forward, desiredDir);
        transform.rotation *= rot;
        hip.SetPositionAndRotation(originalPos, originalRot);
    }

    private void AlignPositionToHip()
    {
        Vector3 originalPos = hip.position;
        transform.position = hip.position;

        Vector3 offset = GetStandUpBoneTransforms()[0].Pos;
        offset.y = 0;
        offset = transform.rotation * offset;
        transform.position -= offset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        hip.position = originalPos;
    }
    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i].Pos = bones[i].localPosition;
            boneTransforms[i].Rotation = bones[i].localRotation;
        }
    }
    private void PopulateAnimationBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                PopulateAnimationBoneTransforms(clip, boneTransforms);
                return;
            }
        }
    }
    private void PopulateAnimationBoneTransforms(AnimationClip clip, BoneTransform[] boneTransforms)
    {
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;

        clip.SampleAnimation(gameObject, 0);
        PopulateBoneTransforms(boneTransforms);


        transform.SetPositionAndRotation(originalPos, originalRot);
    }
    private string StateName()
    {
        return isFacingUp ? faceUpStateName : faceDownStateName;

    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {

        return isFacingUp ? faceUpBones : faceDownBones;


    }
    private void DefaultBehaviour()
    {

    }
}


