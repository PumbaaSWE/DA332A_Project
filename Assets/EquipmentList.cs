using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentList : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform equpmentRoot;
    [SerializeField] private EquipmentSwapper swapper;
    public List<Equipment> equipmentList = new List<Equipment>(); //should be readonly but unity visibility


    private void Start()
    {
        CollectEquipment();

        for (int i = 0; i < equipmentList.Count; i++)
        {
            equipmentList[i].Init(animator);
            equipmentList[i].gameObject.SetActive(false);
        }

        swapper.Raise(equipmentList[0]);
    }

    [MakeButton]
    public void CollectEquipment()
    {
        if(!equpmentRoot) return;
        equipmentList.Clear();
        equpmentRoot.ForEveryChild(t => {
            if (t.TryGetComponent(out Equipment e))
            {
                equipmentList.Add(e);
            }
        });
    }
}
