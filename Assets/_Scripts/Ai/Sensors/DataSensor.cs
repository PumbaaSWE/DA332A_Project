using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

[RequireComponent(typeof(EnemyAI))]

public class DataSensor : MonoBehaviour
{
    EnemyAI linkedAI;
    void Start()
    {
        linkedAI = GetComponent<EnemyAI>();
        DataManager.Instance.Register(this);
    }
}
