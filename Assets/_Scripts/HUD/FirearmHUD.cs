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
            firearm = FindAnyObjectByType<Firearm>();
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
