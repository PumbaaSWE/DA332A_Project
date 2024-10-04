using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitch : MonoBehaviour
{

    [SerializeField] Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void Animate()
    {
        anim.enabled = true;
    }

}
