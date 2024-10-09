using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{

    public enum EHeardSoundCategory
    {
        EFootstep,
        EJump,
        EGunshot,
        EWorldSound
            
    }
    //void Start()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}
    public static HearingManager Instance { get; private set; }

    public List<HearingSensor> AllSensors { get; private set; } = new List<HearingSensor>();

    //EventBinding<PlayerShootEvent> eventBinding;

    //public void OnEnable()
    //{
    //    eventBinding ??= new(OnGunshotEmitted);
    //    EventBus<PlayerShootEvent>.Register(eventBinding);
    //}

    //private void OnDisable()
    //{
    //    EventBus<PlayerShootEvent>.Deregister(eventBinding);
    //}
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple HearingTargetManager found. Destroying" + gameObject.name);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    //public void OnGunshotEmitted(PlayerShootEvent shotEvent)
    //{
       
    //}

    public void Register(HearingSensor sensor)
    {
        AllSensors.Add(sensor);
    }

    public void Deregister(HearingSensor sensor)
    {
        AllSensors.Remove(sensor);
    }
  
    public void OnSoundEmitted(GameObject sourse, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        foreach (var sensor in AllSensors)
        {
            sensor.OnHeardSound(sourse, location, category, intensity);
        }
    }
}
