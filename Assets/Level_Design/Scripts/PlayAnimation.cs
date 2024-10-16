using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string trigger;
    //[SerializeField] bool playOnce = true;
    
    public void Play()
    {
        anim.SetTrigger(trigger);
    }
}
