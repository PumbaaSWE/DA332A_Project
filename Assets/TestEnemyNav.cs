using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyNav : MonoBehaviour
{

    public EnemyNavigation navigation;
    void Start()
    {
        
    }

    public void SetTarget(Vector3 pos)
    {
        navigation.ComputePath(pos);
        //navigation.SetPos(pos);
    }

    public void SetPos(Vector3 pos)
    {
        //navigation.ComputePath(pos);
        navigation.SetPos(pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
