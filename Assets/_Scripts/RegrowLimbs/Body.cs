using System;
using UnityEngine;

public class Body : MonoBehaviour
{

    [SerializeField] private Limb headLimb;
    [SerializeField] private Limb rightArmLimb;
    [SerializeField] private Limb leftArmLimb;
    [SerializeField] private Limb rightLegLimb;
    [SerializeField] private Limb leftLegLimb;

    public bool HasHead { get; private set; }
    public bool HasArms { get; private set; }
    public bool HasLegs { get; private set; }

    //int nrOfLegs = 2;
    //int nrOfArms = 2;

    public event Action StateChanged;

    private void Awake()
    {
        //headLimb.LimbSeveredEvent.AddListener(LooseHead);
        HasHead = HasLegs = HasArms = true;
    }

    private void LooseHead(Limb head)
    {
        HasHead = false;
        StateChanged?.Invoke();
    }

    private void RegrownHead(Limb head)
    {
        HasHead = true;
        StateChanged?.Invoke();
    }

    private void LooseLeg(Limb leg)
    {
        HasLegs = false;
        //nrOfLegs--;
        StateChanged?.Invoke();
    }

    private void RegrownLeg(Limb leg)
    {
        //nrOfLegs++;
        if(CheckLegs())
        {
            HasLegs = true;
            StateChanged?.Invoke();
        }
    }

    private void LooseArm(Limb leg)
    {
        HasArms = false;
        //nrOfArms--;
        StateChanged?.Invoke();
    }

    private void RegrownArm(Limb leg)
    {
        //nrOfArms++;
        if (CheckArms())
        {
            HasArms = true;
            StateChanged?.Invoke();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckLegs()
    {
        return rightLegLimb.IsWhole && leftLegLimb.IsWhole;
    }

    public bool CheckArms()
    {
        return rightArmLimb.IsWhole && leftArmLimb.IsWhole;
    }

    public bool CheckHead()
    {
        return headLimb.IsWhole;
    }


#if UNITY_EDITOR
    [ContextMenu("Add Events..")]
    private void AssignEvents()
    {
        Limb[] limbs = GetComponentsInChildren<Limb>();
        for (int i = 0; i < limbs.Length; i++)
        {
            Limb limb = limbs[i];
            string name = limb.gameObject.name;
            if (name.Contains("Arm"))
            {
                //limb.LimbSeveredEvent.AddListener(LooseArm);
                //limb.LimbSeveredEvent.
                UnityEventExtra.AddButDontDupe(limb.LimbSeveredEvent, LooseArm);
                UnityEventExtra.AddButDontDupe(limb.LimbRegownEvent, RegrownArm);
                //UnityEventTools.AddPersistentListener(limb.LimbSeveredEvent, LooseArm);
                //UnityEventTools.AddPersistentListener(limb.LimbRegownEvent, RegrownArm);
                if (name.Contains("Left"))
                {
                    leftArmLimb = limb;
                }
                else
                {
                    rightArmLimb = limb;
                }
            }
            if (name.Contains("Leg"))
            {
                //UnityEventTools.AddPersistentListener(limb.LimbSeveredEvent, LooseLeg);
                //UnityEventTools.AddPersistentListener(limb.LimbRegownEvent, RegrownLeg);
                UnityEventExtra.AddButDontDupe(limb.LimbSeveredEvent, LooseLeg);
                UnityEventExtra.AddButDontDupe(limb.LimbRegownEvent, RegrownLeg);
                if (name.Contains("Left"))
                {
                    leftLegLimb = limb;
                }
                else
                {
                    rightLegLimb = limb;
                }
            }
            if (name.Contains("Head"))
            {
                //UnityEventTools.AddPersistentListener(limb.LimbSeveredEvent, LooseHead);
                //UnityEventTools.AddPersistentListener(limb.LimbRegownEvent, RegrownHead);
                headLimb = limb;
                UnityEventExtra.AddButDontDupe(limb.LimbSeveredEvent, LooseHead);
                UnityEventExtra.AddButDontDupe(limb.LimbRegownEvent, RegrownHead);
            }
        }
    }

    //public static void AddButDontDupe(UnityEvent<Limb> e, UnityAction<Limb> a)
    //{
    //    int length = e.GetPersistentEventCount();
    //    string name = a.Method.Name;


    //    if(length == 0)
    //    {
    //        UnityEventTools.AddPersistentListener(e, a);
    //        return;
    //    }

    //    for (int i = 0; i < length; i++)
    //    {
    //        if (name.Equals(e.GetPersistentMethodName(i)))
    //        {
    //            //we have dupe
    //            Debug.Log("Listener Method added already: " + e.GetPersistentMethodName(i) + " is " + name);
    //        }
    //        else
    //        {
    //            UnityEventTools.AddPersistentListener(e, a);
    //        }
    //    }


    //}
#endif
}
