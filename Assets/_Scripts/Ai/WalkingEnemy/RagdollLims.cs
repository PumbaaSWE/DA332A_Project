using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;

/*
 * From tutorials:
 * https://www.youtube.com/watch?v=KuMe6Iz8pFI
 * https://www.youtube.com/watch?v=B_NnQQKiw6I
 * 
 */
public class RagdollLims : MonoBehaviour, IDamageble
{

    private class BoneTransform
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; }
    }

    private Rigidbody[] rbs;
    private Animator animator;
    private CharacterController controller;
    private Transform hip;

    private enum RagdollState
    {
        Default,
        Ragdoll,
        StandingUp,
        ResettingBones,
        RegrowLimbs
    };
    private RagdollState state = RagdollState.Default;
    [SerializeField] private float timeToGetUp = 5;
    [SerializeField] private float resetBonesTime = 1;
    [SerializeField] private string standUpStateName = "Stand Up";
    [SerializeField] private string standUpClipName = "Stand Up";
    [SerializeField] private string faceDownStateName = "Standing Up";
    [SerializeField] private string faceDownClipName = "Standing Up";
    private float getUpTimer;
    private float resetTimer;
    private bool isFacingUp;

    private Transform[] bones;
    private BoneTransform[] ragdollBones;
    private BoneTransform[] standUpBones;
    private BoneTransform[] faceDownBones;

    private List<Detachable> detached = new List<Detachable>();

    [SerializeField] bool GOAP;

    private CharacterAgent characterAgent;
    private GOAPPlanner planner;
    EnemyAI enemyAi;

    AwarenessSystem senssors;
    Goal_Stalk_W goal_Stalk_W;
    Health health;
    void Awake()
    {   
        goal_Stalk_W = GetComponent<Goal_Stalk_W>();
        health = GetComponent<Health>();
        senssors = GetComponent<AwarenessSystem>();
        enemyAi = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        hip = animator.GetBoneTransform(HumanBodyBones.Hips);


        rbs = hip.GetComponentsInChildren<Rigidbody>();
        bones = hip.GetComponentsInChildren<Transform>();
        ragdollBones = new BoneTransform[bones.Length];
        standUpBones = new BoneTransform[bones.Length];
        faceDownBones = new BoneTransform[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            ragdollBones[i] = new BoneTransform();
            standUpBones[i] = new BoneTransform();
            faceDownBones[i] = new BoneTransform();
        }

        PopulateAnimationBoneTransforms(standUpClipName, standUpBones);
        PopulateAnimationBoneTransforms(faceDownClipName, faceDownBones);




        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint joint in joints)
        {
            joint.enableProjection = true;
        }
    }

    private void Start()
    {
        // do a generic. This is temporary
        if (GOAP)
        {
            characterAgent = GetComponent<CharacterAgent>();
            planner = GetComponent<GOAPPlanner>();
        }
      

        DisableRagdoll();
    }
    // Update is called once per frame
    void Update()
    {
        VisonUpdate();

        switch (state)
        {
            case RagdollState.Default:
                DefaultBehaviour();
                break;
            case RagdollState.Ragdoll:
                RagdollBehaviour();
                break;
            case RagdollState.StandingUp:
                StandingUpBehaviour();
                break;
            case RagdollState.ResettingBones:
                ResettingBonesBehaviour();
                break;
            case RagdollState.RegrowLimbs:
                RegrowLimbsBehaviour();
                break;
            default:
                break;
        }
    }
    public bool IsLeggDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.legg && detachable.detached)
            {
                return true;
            }
        }

        return false;
    }
    public bool IsHeadDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.head && detachable.detached)
            {
                return true;
            }
        }

        return false;
    }

    private void RegrowLimbsBehaviour()
    {
        if (detached.Count <= 0)
        {

            AlignRotationToHip();
            AlignPositionToHip();

            PopulateBoneTransforms(ragdollBones);

            state = RagdollState.ResettingBones;

            resetTimer = 0;
            return;
        }
        detached.RemoveAll(x => !x.DetatchedLimb());
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
            //bones[i].localPosition = pos;
            Quaternion rot = Quaternion.Lerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, t);
            bones[i].SetLocalPositionAndRotation(pos, rot);
            //bones[i].localPosition = pos;
        }

        if (resetTimer >= resetBonesTime)
        {
            state = RagdollState.StandingUp;
            DisableRagdoll();
            animator.Play(StateName(), 0, 0);
        }
    }

    private void StandingUpBehaviour()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
        {
            state = RagdollState.Default;
            // change this later
            // give back controll to Ai
          
            

                planner.deactivate = false;
                characterAgent.isCrawling = false;
            
        }
    }

    private void DisableRagdoll()
    {
      
        
            health.Heal(health.MaxHealth);
            characterAgent.SetAgentActive(true);
            planner.deactivate = false;
        



        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }
        animator.enabled = true;
        controller.enabled = true;
    }

    private void EnableRagdoll()
    {
       
        
            characterAgent.SetAgentActive(false);
            planner.deactivate = true;
        


        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = false;
        }
        animator.enabled = false;
        controller.enabled = false;
    }

    private void DefaultBehaviour()
    {

    }
    private void VisonUpdate()
    {
        if (IsHeadDetached())
        {
            characterAgent.isBlind = true;

        }
        else
        {
            characterAgent.isBlind = false;
        }
    }

    private void RagdollBehaviour()
    {
        getUpTimer -= Time.deltaTime;
        if (getUpTimer < 3 && IsLeggDetached())
        {///////////////////////////////////////
            characterAgent.isCrawling = true;
            DisableRagdoll();

            planner.deactivate = false;
            characterAgent.SetAgentActive(true);

        }



        if (getUpTimer < 0)
        {
            isFacingUp = hip.forward.y > 0;

            foreach (var detachable in detached)
            {
                detachable.Regrow(2);
            }



            //AlignRotationToHip();
            //AlignPositionToHip();

            //PopulateBoneTransforms(ragdollBones);

            state = RagdollState.RegrowLimbs;
            resetTimer = 0;
        }
    }
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    { 
        goal_Stalk_W.prio -= 30;
        health.Damage( damage);
        if(health.Value <= 0)
        {
            TriggerRagdoll(direction, point);
        }
       
    }
    public void TriggerRagdoll(Vector3 force, Vector3 point)
    {

     
        Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        rb.AddForceAtPosition(force, point, ForceMode.Impulse);

        if (rb.TryGetComponent(out Detachable detachable))
        {
            detachable.Detatch();
            detached.Add(detachable);
        }
        if(IsLeggDetached())
        {
            EnableRagdoll();
        }
        state = RagdollState.Ragdoll;
        getUpTimer = timeToGetUp;
    }

    private void AlignRotationToHip()
    {
        Vector3 originalPos = hip.position;
        Quaternion originalRot = hip.rotation;
        Vector3 desiredDir = hip.up * (isFacingUp ? -1 : 1);
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
        return isFacingUp ? standUpStateName : faceDownStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return isFacingUp ? standUpBones : faceDownBones;
    }

}
