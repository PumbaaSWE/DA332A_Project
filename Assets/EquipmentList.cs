using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentList : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform equpmentRoot;
    [SerializeField] private EquipmentSwapper swapper;
    [SerializeField] private int current = 0;
    public List<Equipment> equipmentList = new List<Equipment>(); //should be readonly but unity visibility

    private void Start()
    {

        if(equipmentList == null)
        {
            equipmentList = new List<Equipment>();
            CollectEquipment();
        }

        for (int i = 0; i < equipmentList.Count; i++)
        {
            equipmentList[i].Init(animator);
            equipmentList[i].gameObject.SetActive(false);
        }
        swapper.Raise(equipmentList[current]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NextEquipment();
        }
    }

    [MakeButton(true)]
    public void NextEquipment()
    {
        current++;
        current %= equipmentList.Count;
        swapper.Raise(equipmentList[current]);
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
