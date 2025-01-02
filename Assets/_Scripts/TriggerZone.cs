using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    //public UnityEvent<Transform> TriggerEnter;
    //public UnityEvent<Transform> TriggerExit;

    [SerializeField] LayerMask triggerMask;

    [Tooltip("set to 0 for always trigger")]
    public int maxTriggers = 1;
    public float resetTriggerTime = 1;
    private float cooldown;
    public enum TriggerBehaviour { OnEnter, OnExit, Both }
    public TriggerBehaviour triggerBehaviour = TriggerBehaviour.OnEnter;
    public UnityEvent<Transform> InZoneTriggered;

    private Collider zone;
    [SerializeField] private bool debugDraw = true;


    private void Awake()
    {
        zone = GetComponent<Collider>();
        zone.isTrigger = true;
        //gameObject.layer = triggerMask;
        gameObject.layer = 2;
    }
    
    public void TriggerTheZone(Transform trigger)
    {
        if (!enabled) return;
        if (cooldown > 0) return;
        cooldown = resetTriggerTime;
        maxTriggers--;
        InZoneTriggered?.Invoke(trigger);
        if (maxTriggers == 0)
        {
            enabled = false;
            zone.enabled = false;
        }
    }

    private void Update()
    {
        //cooldown -= Time.deltaTime;
        cooldown = cooldown < 0 ? 0 : cooldown - Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent(out Player _))
            return;
        if (triggerBehaviour != TriggerBehaviour.OnExit)
        {
            TriggerTheZone(other.transform);
        }
        //TriggerEnter?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.TryGetComponent(out Player _))
            return;
        if (triggerBehaviour != TriggerBehaviour.OnEnter)
        {
            TriggerTheZone(other.transform);
        }
        //TriggerExit?.Invoke(other.transform);
    }

    private void OnValidate()
    {
        GetComponent<Collider>().excludeLayers = ~triggerMask;
        GetComponent<Collider>().includeLayers = triggerMask;
        GetComponent<Collider>().layerOverridePriority = 1;
        zone = GetComponent<Collider>();
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw) return;
        if (!zone) return;
            //Gizmos.matrix = transform.localToWorldMatrix;
        var m = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.cyan;
        var box = zone as BoxCollider;
        Gizmos.DrawWireCube(box.center, box.size);
        


        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one*2);
        Gizmos.matrix = m;
    }
}
