using UnityEngine;
[CreateAssetMenu(fileName = "New FirearmData", menuName = "ScriptableObjects/FirearmData", order = 104)]
public class FirearmData : ScriptableObject
{

    public int magSize = 30;
    public bool canChamberRound = true;
    public AmmoType ammoType;
    public float roundsPerMinute = 600;
    //public int bulletsPerShot = 1;

    public int ChamberRound => canChamberRound ? 1 : 0;
    public int MaxAmmo => magSize + (canChamberRound ? 1 : 0);

    public float FireTime { get; private set; }

    private void OnEnable()
    {
        FireTime = 60.0f / roundsPerMinute;
    }
}