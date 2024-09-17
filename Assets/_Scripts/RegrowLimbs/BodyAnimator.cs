using UnityEngine;

public class BodyAnimator : MonoBehaviour
{
    public Body body;
    public Animator animator;
    readonly int hasLegsHash = Animator.StringToHash("HasLegs");

    // Start is called before the first frame update
    void Awake()
    {
        body.StateChanged += CheckState;
    }

    private void Start()
    {
        CheckState();
    }

    private void CheckState()
    {
        animator.SetBool(hasLegsHash, body.HasLegs);
    }

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
