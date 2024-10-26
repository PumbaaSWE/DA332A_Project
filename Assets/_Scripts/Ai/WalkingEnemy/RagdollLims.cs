using System;
using System.Collections;
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
public class RagdollLims : MonoBehaviour/*, IDamageble*/
{
    public float deahtHealth = 600;
    public GameObject enemy;

    private class BoneTransform
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; }
    }

    private Rigidbody[] rbs;
    private Animator animator;
    private CharacterController controller;
    private Transform hip;

  
    public enum RagdollState
    {
        Default,
        Ragdoll,
        StandingUp,
        ResettingBones,
        RegrowLimbs
    };
    public RagdollState state = RagdollState.Default;
    [SerializeField] private float timeToGetUp = 5;
    [SerializeField] private float resetBonesTime = 1;
    [SerializeField] private string standUpStateName = "Stand Up";
    [SerializeField] private string standUpClipName = "Stand Up";
    [SerializeField] private string faceDownStateName = "Standing Up";
    [SerializeField] private string faceDownClipName = "Standing Up";
    [SerializeField] private float startRegrow;
    private float getUpTimer;
    private float resetTimer;
    private bool isFacingUp;

    private float resetSingelTimer;

    private Transform[] bones;
    private BoneTransform[] ragdollBones;
    private BoneTransform[] standUpBones;
    private BoneTransform[] faceDownBones;

    private List<Detachable> detached = new List<Detachable>();



    //private CharacterAgent characterAgent;
    //private GOAPPlanner planner;
    //EnemyAI enemyAi;

    //AwarenessSystem senssors;
    //Goal_Stalk_W goal_Stalk_W;
    Health health;
    FSM fSM;

   
    void Awake()
    {   
        fSM = GetComponent<FSM>();
        //goal_Stalk_W = GetComponent<Goal_Stalk_W>();
        health = GetComponent<Health>();
        //senssors = GetComponent<AwarenessSystem>();
        //enemyAi = GetComponent<EnemyAI>();
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
        DisableRagdoll();
    }
    // Update is called once per frame
    void Update()
    {
        //VisonUpdate();
        Death();

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
    public bool IsLegDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.leg && detachable.detached)
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
    public bool IsArmDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.arm && detachable.detached)
            {
                return true;
            }
        }
        
        return false;
    }
    //public bool IsAllArmsDetached()
    //{
    //    int nrAmrs = 0;
    //    int nrAmrsDetached = 0;
    //    foreach (var detachable in detached)
    //    {
    //        if (detachable.arm)
    //        {
    //            nrAmrs++;
    //        }
    //        if (detachable.arm && detachable.detached)
    //        {
    //            nrAmrsDetached++;   
    //        }
    //    }
    //    if(nrAmrsDetached == nrAmrs)
    //    {
    //        nrAmrs = 0;
    //        nrAmrsDetached = 0;
    //        return true;
    //    }
    //    nrAmrs = 0;
    //    nrAmrsDetached = 0;
    //    return false;
    //}

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
        detached.RemoveAll(x => !x.DetatchedLimb() && !x.growing);
    }

    private void ResetingSingelDetached()
    {
        if (detached.Count <= 0)
        {
            PopulateBoneTransforms(ragdollBones);
            resetSingelTimer = 0;

            ResetBone();
        }
        detached.RemoveAll(x => !x.DetatchedLimb() && !x.growing);

    }

    void ResetBone()
    {
        resetSingelTimer += Time.deltaTime;
        float t = resetSingelTimer / resetBonesTime;
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
           
            if(fSM.isCrawling)
            {
                state = RagdollState.StandingUp;
                DisableRagdoll();

                animator.Play(StateName(), 0, 0);
                //fSM.isCrawling = false;
                //fSM.agentStatehit = FSM.AgentHit.Normal;
                fSM.SetAgentActive(true);

                StartCoroutine(PlayAndWaitForAnimation());

            }
        }
    }
    private IEnumerator PlayAndWaitForAnimation()
    {
        animator.Play(StateName(), 0, 0);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        //animator.Play("Base Layer.Idle");
        fSM.isCrawling = false;
        //fSM.agentStatehit = FSM.AgentHit.Normal;
        //fSM.SetAgentActive(true);
    }
    private void StandingUpBehaviour()
    {
        //if(fSM.isCrawling)
        {
                
           
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
            {
                state = RagdollState.Default;
               
                //fSM.isCrawling = false;
                //fSM.agentStatehit = FSM.AgentHit.Normal;
            }

        }
       
    }

    private void DisableRagdoll()
    {
      
        
        health.Heal(health.MaxHealth);
        //if (GOAP)
        //{
        //    characterAgent.SetAgentActive(true);
        //    planner.deactivate = false;

        //}
        //else
        {

          
        }
        fSM.SetAgentActive(true);
       

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }

        animator.enabled = true;
        controller.enabled = true;

       
    }

    public void EnableRagdoll()
    {
       
        //if(GOAP)
        //{
        //    characterAgent.SetAgentActive(false);
        //    planner.deactivate = true;
        //}
        //else
        {
            fSM.SetAgentActive(false);
        }




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
    //private void VisonUpdate()
    //{
    //    if (GOAP)
    //    {
    //        if (IsHeadDetached())
    //        {
    //            characterAgent.isBlind = true;

    //        }
    //        else
    //        {
    //            characterAgent.isBlind = false;
    //        }

    //    }
       
    //}

    private void RagdollBehaviour()
    {
        getUpTimer -= Time.deltaTime;
        if (getUpTimer < 3.5)
        {
            DisableRagdoll();
            animator.CrossFade("crawl", .2f);

        }
        if (getUpTimer < 3 /*&& IsLegDetached()*/)
        {///////////////////////////////////////
          
           
            
                fSM.isCrawling = true;
                //////////
                //fSM.SetAgentActive(true);
                /////////
            
        }

        if (getUpTimer < 0)
        {
            isFacingUp = hip.forward.y > 0;

            foreach (var detachable in detached)
            {
                //if(detachable.growing == false)
                {
                    if(detachable.leg)
                    {
                        detachable.Regrow(2);

                    }
                }
            }
            //AlignRotationToHip();
            //AlignPositionToHip();
            //PopulateBoneTransforms(ragdollBones);

            state = RagdollState.RegrowLimbs;
            resetTimer = 0;
        }
    }
   

    // Coroutine som väntar 2 sekunder innan den fortsätter
   
    public void Regrow(bool arm)
    {
        StartCoroutine(StartRegrow(arm));
    }

    IEnumerator StartRegrow(bool arm)
    {
        yield return new WaitForSeconds(2);

        //isFacingUp = hip.forward.y > 0;

        foreach (var detachable in detached)
        {
            if(arm)
            {
                if(detachable.arm)
                {
                    detachable.Regrow(2);
                    StartCoroutine(ResetArms());

                }
            }
            else if(!arm)
            {
                if (detachable.head)
                {
                    detachable.Regrow(2);
                }
            }
        }
        ResetingSingelDetached();

        //state = RagdollState.RegrowLimbs;
        resetSingelTimer = 0;
        //AlignRotationToHip();
        //AlignPositionToHip();
        //PopulateBoneTransforms(ragdollBones);

        //state = RagdollState.RegrowLimbs;
        //resetTimer = 0;
    }
    IEnumerator ResetArms()
    {
      
        yield return new WaitForSeconds(2f);
        fSM.isArmles = false;

    }
    //public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    //{ 
    //    health.Damage( damage);
    //    deahtHealth -= damage;
    //    //if (GOAP)
    //    //{
    //    //    goal_Stalk_W.prio -= 30;


    //    //}
    //    if(health.Value <= 0)
    //    {

    //        //if (GOAP)
    //        //{
    //        //    characterAgent.isCrawling = false;
    //        //    planner.deactivate = false;
    //        //    characterAgent.SetAgentActive(false);
    //        //}
    //        //else
    //        {
    //            //fSM.isCrawling = false;
    //            //fSM.SetAgentActive(false);
    //        }
    //        //TriggerRagdoll(direction, point);
    //    }

    //}

    public void Death()
    {
        if (deahtHealth <= 0)
        {
            EnableRagdoll();
            state = RagdollState.Ragdoll;
            TakeDMG(new Vector3(10, 10, 10), new Vector3(0, 1, 0));
            Destroy(enemy, 1.5f);           
        }
    }

    public void TakeDMG(Vector3 force, Vector3 point)
    {
        Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        rb.AddForceAtPosition(force, point, ForceMode.Impulse);

        if (rb.TryGetComponent(out Detachable detachable))
        {
            detachable.Detatch();
            detached.Add(detachable);
        }
        if (IsLegDetached())
        {
            TriggerRagdoll();
        }
    }
    public void TriggerRagdoll()
    {

        //Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        //rb.AddForceAtPosition(force, point, ForceMode.Impulse);

        //if (rb.TryGetComponent(out Detachable detachable))
        //{
        //    detachable.Detatch();
        //    detached.Add(detachable);
        //}

        //if(IsLegDetached())
        {
            EnableRagdoll();
            state = RagdollState.Ragdoll;
            fSM.SetAgentActive(false);
            //fSM.isCrawling = true;
        }

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
