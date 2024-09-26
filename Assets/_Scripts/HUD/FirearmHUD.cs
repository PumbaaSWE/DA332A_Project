using TMPro;
using UnityEngine;

public class FirearmHUD : MonoBehaviour
{

    Firearm firearm;
    [SerializeField] TMP_Text text;


    public void SetFirearm(Firearm firearm)
    {
        this.firearm = firearm;
    }


    void Start()
    {
        if (!firearm)
        {
            //firearm = FindAnyObjectByType<Firearm>();
            FindEquippedFireArm();
        }
    }
    void FindEquippedFireArm()
    {
        GameObject playerParent = GameObject.Find("Player");
        if (playerParent) {
            firearm = playerParent.GetComponentInChildren<Firearm>();
            TooltipUtil.Display("Press left click to shoot", 10.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (firearm)
        {
            text.text = firearm.LoadedAmmo.ToString();
        }
    }
}
