
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/*
 * From tutorials:
 * https://www.youtube.com/watch?v=KuMe6Iz8pFI
 * https://www.youtube.com/watch?v=B_NnQQKiw6I
 * 
 */
public class RagdollLims : MonoBehaviour
{
  
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
        StartCrawl,
        StandingUp,
        ResettingBones,
        RegrowLimbs
    };
    public RagdollState state = RagdollState.Default;
    [SerializeField] private float timeToGetUp = 5;
    [SerializeField] private float resetBonesTime = 1;
    private float timeResetBonesTime = .5f;

    [SerializeField] private string standUpStateName = "Stand Up";
    [SerializeField] private string standUpClipName = "Stand Up";
    [SerializeField] private string faceDownStateName = "Standing Up";
    [SerializeField] private string faceDownClipName = "Standing Up";

    [SerializeField] private string getUpStateName = "Getting Up";
    [SerializeField] private string getUpClipName = "Getting Up";
    [SerializeField] private string rollStateName = "Rolling";
    [SerializeField] private string rollClipName = "Rolling";

    [SerializeField] private float startRegrow;
    private float getUpTimer;
    private float resetTimer;
    private bool isFacingUp;

    //private float resetSingelTimer;
    private float elapsedResetBonesTime;
   

    private Transform[] bones;
    private BoneTransform[] ragdollBones;
    private BoneTransform[] standUpBones;
    private BoneTransform[] faceDownBones;

    private BoneTransform[] gettingUpBones;
    private BoneTransform[] rollBones;

    private List<Detachable> detached = new List<Detachable>();

    bool ragToCrawl;


   
    Health health;
    FSM fSM;
    public float blendDuration = 1.5f;

    void Awake()
    {   
        fSM = GetComponent<FSM>();
    
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

        gettingUpBones = new BoneTransform[bones.Length];
        rollBones = new BoneTransform[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            ragdollBones[i] = new BoneTransform();
            standUpBones[i] = new BoneTransform();
            faceDownBones[i] = new BoneTransform();

            gettingUpBones[i] = new BoneTransform();
            rollBones[i] = new BoneTransform();
        }

        PopulateAnimationBoneTransforms(standUpClipName, standUpBones);
        PopulateAnimationBoneTransforms(faceDownClipName, faceDownBones);

        PopulateAnimationBoneTransforms(getUpClipName, gettingUpBones);
        PopulateAnimationBoneTransforms(rollClipName, rollBones);




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
            case RagdollState.StartCrawl:
                CrawlingUpBehaviour();
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

    //private void RegrowLimbsBehaviour()
    //{
    //    if (detached.Count <= 0)
    //    {

    //        AlignRotationToHip();
    //        AlignPositionToHip(true);

    //       PopulateBoneTransforms(ragdollBones);

    //        state = RagdollState.ResettingBones;

    //        resetTimer = 0;
    //        return;
    //    }
    //    detached.RemoveAll(x => !x.DetatchedLimb() && !x.growing);
    //}
    private void RegrowLimbsBehaviour()
    {
        ragToCrawl = true;
        AlignRotationToHip(); 
        AlignPositionToHip(true); 

       
        detached.RemoveAll(detachable => !detachable.DetatchedLimb() && !detachable.growing);

     
        if (detached.Count <= 0)
        {
            PopulateBoneTransforms(ragdollBones);
            state = RagdollState.ResettingBones;
            resetTimer = 0;
        }
    }
    private void ResetingSingelDetached()
    {
        if (detached.Count <= 0)
        {
            PopulateBoneTransforms(ragdollBones);
            //resetSingelTimer = 0;

            //ResetBone();
        }
        detached.RemoveAll(x => !x.DetatchedLimb() && !x.growing);

    }

    //void ResetBone()
    //{
    //    resetSingelTimer += Time.deltaTime;
    //    float t = resetSingelTimer / resetBonesTime;
    //    int length = bones.Length;
    //    BoneTransform[] standUpBones = GetStandUpBoneTransforms();
    //    for (int i = 0; i < length; i++)
    //    {
    //        Vector3 pos = Vector3.Lerp(ragdollBones[i].Pos, standUpBones[i].Pos, t);
    //        //bones[i].localPosition = pos;
    //        Quaternion rot = Quaternion.Lerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, t);
    //        bones[i].SetLocalPositionAndRotation(pos, rot);
    //        //bones[i].localPosition = pos;
    //    }

    //}
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
            
                fSM.SetAgentActive(false);
                state = RagdollState.StandingUp;
                DisableRagdoll();
               
                animator.Play(StateName(), 0, 0);
                fSM.isCrawling = false;

            }
        }
    }
  
    private void CrawlingUpBehaviour()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
        {

            fSM.isCrawling = true;
            
        }
    }
 
    private void StandingUpBehaviour()
    {
        //if(fSM.isCrawling)
        {
                
           
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateName()))
            {
                state = RagdollState.Default;
                fSM.SetAgentActive(true);
                //fSM.isCrawling = false;
                //fSM.limbStatehit = FSM.AgentHit.Normal;
            }

        }
       
    }

    private void DisableRagdoll()
    {
      
        
        health.Heal(health.MaxHealth);
      

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }

        animator.enabled = true;
        controller.enabled = true;
        

    }

    public void EnableRagdoll()
    {

            fSM.SetAgentActive(false);


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
   
    void GetUpCrawl()
    {
        //isFacingUp = hip.forward.y > 0;
        //detached.RemoveAll(x => !x.DetatchedLimb() && !x.growing);
        //if (isFacingUp)
        {
            //PopulateBoneTransforms(ragdollBones);
           
        }
        

        elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage =   timeResetBonesTime/ elapsedResetBonesTime;
        BoneTransform[] standUpBones = GetStandUpBoneCrawlTransforms();
        int length = bones.Length;
        for (int i = 0; i < length; i++)
        {
            //bones[i].localPosition = Vector3.Lerp(ragdollBones[i].Pos, standUpBones[i].Pos, elapsedPercentage);
            //bones[i].localRotation = Quaternion.Slerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, elapsedPercentage);

            Vector3 pos = Vector3.Lerp(ragdollBones[i].Pos, standUpBones[i].Pos, elapsedPercentage);

           // Quaternion rot = Quaternion.Slerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, elapsedPercentage);
             Quaternion rot = Quaternion.Lerp(ragdollBones[i].Rotation, standUpBones[i].Rotation, elapsedPercentage);

            bones[i].SetLocalPositionAndRotation(pos, rot);


        }


        if (elapsedPercentage >= 1 && !ragToCrawl)
        {
            DisableRagdoll();           
            animator.Play(StateNameTwo(), 0, 0);
            fSM.SetAgentActive(true);
            ragToCrawl = true;

        }
      
    }
    private void RagdollBehaviour()
    {
        getUpTimer -= Time.deltaTime;

        if (ragToCrawl)
        {
            AlignRotationToHip();
            AlignPositionToHip(false);
            ragToCrawl = false;
        }
       

        if (getUpTimer < 7.5f && getUpTimer >= 7.0f)
        {
            isFacingUp = hip.forward.y > 0;
            GetUpCrawl();
         
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StateNameTwo()) && getUpTimer < 7)
        {
            fSM.isCrawling = true;
            fSM.SetAgentActive(true);

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
        //resetSingelTimer = 0;


        //state = RagdollState.RegrowLimbs;

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
   

    public void Death()
    {
            EnableRagdoll();
            state = RagdollState.Ragdoll;
            TakeDMG(new Vector3(10, 10, 10), new Vector3(0, 1, 0));
            Destroy(enemy, 1.5f);           
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
            
            //fSM.isCrawling = true;
        }

        getUpTimer = timeToGetUp;
        elapsedResetBonesTime = 0;
    }

    private void AlignRotationToHip()
    {
        Vector3 originalPos = hip.position;
        Quaternion originalRot = hip.rotation;
        Vector3 desiredDir = hip.up * (isFacingUp ? -1 : 1);
        desiredDir.y = 0;
        //desiredDir.z *= -1;
        //desiredDir.Normalize();

        Quaternion rot = Quaternion.FromToRotation(transform.forward, desiredDir);
        transform.rotation *= rot;

        hip.SetPositionAndRotation(originalPos, originalRot);

    }

    //private void AlignRotationToHip()
    //{
    //    Vector3 originalPos = hip.position;
    //    Quaternion originalRot = hip.rotation;

    //    Vector3 desiredDir = hip.up * (isFacingUp ? -1 : 1);
    //    desiredDir.y = 0;
    //    //desiredDir.z *= -1;
    //    desiredDir.Normalize();

    //    Quaternion targetRotation = Quaternion.LookRotation(desiredDir, Vector3.up);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

    //    hip.SetPositionAndRotation(originalPos, originalRot);
    //}

    private void AlignPositionToHip(bool stand)
    {
        Vector3 originalPos = hip.position;
        transform.position = hip.position;

        Vector3 offset = GetStandUpBoneTransforms()[0].Pos;

        if(!stand)
        {
            offset = GetStandUpBoneCrawlTransforms()[0].Pos;
        }

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
    private string StateNameTwo()
    {
        //isFacingUp = hip.forward.y > 0;

        return isFacingUp ? getUpStateName : rollStateName;
    }

    private BoneTransform[] GetStandUpBoneCrawlTransforms()
    {            
        //isFacingUp = hip.forward.y > 0;

        return isFacingUp ? gettingUpBones : rollBones;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return isFacingUp ? standUpBones : faceDownBones;
    }

}
