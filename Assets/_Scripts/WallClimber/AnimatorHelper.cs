using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    [SerializeField] Animator animator;
    int speesHash = Animator.StringToHash("Speed");
    [SerializeField] WallClimber wallClimber;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(speesHash, Mathf.Min(1, wallClimber.Speed));
    }
}
