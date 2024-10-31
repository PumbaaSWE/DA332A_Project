
using System.Linq;
using UnityEngine;

using static Regrow;

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
    private void Awake()
    {

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
        Debug.Log("align");
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
            Debug.Log($"Ben {i}: Pos={standUpBones[i].Pos}, Rot={standUpBones[i].Rotation}");
        }

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
            animator.Play(StateName(), 0, 0);
        }
    }

    private void CrawlUpBehaviour()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
        {
            state = RagdollState.Default;
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
    }

    public void EnableRagdoll()
    {
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


