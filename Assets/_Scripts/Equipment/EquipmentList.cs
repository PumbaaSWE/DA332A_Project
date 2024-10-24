using System.Collections.Generic;
using UnityEngine;

public class EquipmentList : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform equpmentRoot;
    [SerializeField] private EquipmentSwapper swapper;
    [SerializeField] private int current = 0;
//    [SerializeField] private List<Equipment> equipmentList = new List<Equipment>(); //should be readonly but unity visibility
    [SerializeField] private int numSlots = 6;
    [SerializeField] private Equipment[] equipmentSlots; //should be readonly but unity visibility

    private Equipment toDrop;
//    public List<Equipment> Equipments => equipmentList;

    private void Start()
    {
        //equipmentSlots = new Equipment[numSlots];
        if(equipmentSlots == null || equipmentSlots.Length != numSlots)
        {
            CollectEquipment();
        }

        //if (equipmentList == null)
        //{
        //    equipmentList = new List<Equipment>();
        //    CollectEquipment();
        //}

        //for (int i = 0; i < equipmentList.Count; i++)
        //{
        //    equipmentList[i].Init(animator);
        //    equipmentList[i].gameObject.SetActive(false);
        //}
        swapper.Raise(equipmentSlots[current]);

        swapper.OnLowerCompleted += Swapper_OnLowerCompleted;

    }

    private void Swapper_OnLowerCompleted(Equipment obj)
    {
        if(toDrop != null)
        {
            Equipment _ = Instantiate(toDrop.EquipmentData.OnDropPrefab, transform.position + transform.forward, Quaternion.identity);
            //if(eq.TryGetComponent(out FirearmPickUp fpu))
            //{
            //    fpu.LoadedAmmo = toDrop.GetComponent<Firearm>().LoadedAmmo;
            //}
            Destroy(toDrop.gameObject);
            toDrop = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NextEquipment(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NextEquipment(false);
            //PreviousEquipment();
        }
    }

    public bool HasEquipment(Equipment equipment)
    {
        int i = equipment.EquipmentData.Slot;
        return equipmentSlots[i] == equipment;
        //return equipmentList.Contains(equipment);
    }

    public void AddEquipment(Equipment equipment)
    {
        int i = equipment.EquipmentData.Slot;
        if(i < 0 || i >= numSlots)
        {
            Debug.LogError("EquipmentList - EquipmentData.Slot is not in the correct range, max is :" + (numSlots -1) +", check the scriptableObj: " + equipment.EquipmentData.name);
            return;
        }
        toDrop = equipmentSlots[i];
        equipment.Init(animator);
        equipmentSlots[i] = equipment;

    }

    //next equipment that is not null
    public void NextEquipment(bool up)
    {
        int d = up ? 1 : -1;
        int c = 0;
        while (c++ < numSlots)
        {
            current += d;
            if(current == numSlots) current = 0;
            if(current == -1) current = numSlots-1;
            //current = (current + d) % numSlots;
            if (equipmentSlots[current] != null)
            {
                c = numSlots;
            }
        }
        swapper.Raise(equipmentSlots[current]);
    }

    public void SelectWeapon()
    {
        current = (current == 1) ? 0 : 1;
        swapper.Raise(equipmentSlots[current]);
    }

    public void SecondaryWeapon()
    {
        current = (current == 0) ? 1 : 0;
        swapper.Raise(equipmentSlots[current]);
    }


    //public void NextEquipment()
    //{
    //    current++;
    //    current %= equipmentList.Count;
    //    swapper.Raise(equipmentList[current]);
    //}

    //public void PreviousEquipment()
    //{
    //    current--;
    //    current %= equipmentList.Count;
    //    swapper.Raise(equipmentList[current]);
    //}

    [MakeButton]
    public void CollectEquipment()
    {
        if (!equpmentRoot)
        {
            Debug.LogError("EquipmentList - equpmentRoot == null - please assign a Transform in the inspector");
            return;
        }
        //equipmentList.Clear();
        //equipmentSlots ??= new Equipment[numSlots];
        if(equipmentSlots == null || equipmentSlots.Length != numSlots)
        {
            equipmentSlots = new Equipment[numSlots];
        }

        equpmentRoot.ForEveryChild(t => {
            if (t.TryGetComponent(out Equipment e))
            {
                AddEquipment(e);
            }
        });
    }
}
