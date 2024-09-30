using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDEBUGScript : MonoBehaviour
{
    public int HP;
    // Start is called before the first frame update
    void Start()
    {
        HP = 99;
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP < 0) { HP = 0; }
    }
    public void RecieveHP(int amount)
    {
        HP += amount;
        if (HP > 100) { HP = 100; }
    }
    ////Update is called once per frame
    //void Update()
    //{

    //}
}
