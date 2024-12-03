using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class RagdollToAnimation : MonoBehaviour
{


    private class BoneTransform
    {
        public Vector3 Pos { get; set; }
        public Quaternion Rotation { get; set; }
    }

    private Rigidbody[] rbs;
    private Animator animator;
    private Transform hip;

    private bool isFacingUp;
    private float resetTimer;

    private Transform[] bones;
    private BoneTransform[] ragdollBones;
    private BoneTransform[] faceUpBones;
    private BoneTransform[] faceDownBones;


    private enum PedestrianState
    {
        Ragdoll,
        Animating,
        ResettingBones,
    };
    private PedestrianState state = PedestrianState.Animating;
   // [SerializeField] private float timeToGetUp = 5;
    [SerializeField] private float resetBonesTime = 1;
    [SerializeField] private string faceUpStateName = "Stand Up";
    [SerializeField] private string faceUpClipName = "Stand Up";
    [SerializeField] private string faceDownStateName = "Standing Up";
    [SerializeField] private string faceDownClipName = "Standing Up";

    Action callback;

    void Awake()
    {
        animator = GetComponent<Animator>();
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

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PedestrianState.ResettingBones:
                ResettingBonesBehaviour();
                break;
            default:
                break;
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
            state = PedestrianState.Animating;
            //DisableRagdoll();
            animator.enabled = true;
            //animator.PlayInFixedTime(StateName(), 0);
            animator.Rebind();
            animator.Update(.5f);
            animator.Play(StateName(), 0, 0);
            callback?.Invoke();
        }
    }

    public void BeginAnimating(Action action)
    {
        AlignRotationToHip();
        AlignPositionToHip();
        PopulateBoneTransforms(ragdollBones);
        state = PedestrianState.ResettingBones;
        resetTimer = 0;
        callback = action;
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
        return isFacingUp ? faceUpStateName : faceDownStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return isFacingUp ? faceUpBones : faceDownBones;
    }
}
