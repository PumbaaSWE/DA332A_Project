using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class FlareThrower : MonoBehaviour
{
    [SerializeField] private Transform lookDir;
    [SerializeField] private Flare flarePrefab;
    [SerializeField] private Equipment flareEqipment;
    [SerializeField] private Animator animator;
    [SerializeField] private float throwAngle = 15;
    [SerializeField] private float collideDist = 1.5f;
    [SerializeField] private float throwForce = 15;
    [SerializeField] private float torqueForce = 1;

    [SerializeField] private int numFlares;
    [SerializeField] private int maxNumFlares = 5;

    public int NumFlares { get { return numFlares; } set { SetFlares(value); } }
    public int MaxNumFlares { get { return maxNumFlares; } set { maxNumFlares = Mathf.Min(value, 0); } }

    private AnimationEvents animationEvents;
    private EquipmentSwapper equipmentSwapper;
    private bool hasThrownFlare;

    [SerializeField] private PlayerInput playerInput;
    private InputAction action;
    private string key = "<nope>";
    readonly int throwHash = Animator.StringToHash("FireBool");
    bool throwing = false;
    Equipment prev;
    // Start is called before the first frame update
    void Start()
    {
        action = playerInput.actions.FindAction("F");
        key = "[" + action.bindings.First().ToDisplayString() + "]";
        action.performed += Action_performed;


        equipmentSwapper = GetComponentInChildren<EquipmentSwapper>();
        animationEvents = GetComponentInChildren<AnimationEvents>();
        if (animationEvents)
        {
            animationEvents.throwEvent.AddListener(ThrowFlare);
        }

    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (numFlares > 0 && !throwing)
        {

            prev = equipmentSwapper.Current;
            equipmentSwapper.Raise(flareEqipment);
            animator.SetBool(throwHash, true);
            throwing = true;
            StartCoroutine(ResetThrowing());
        }
    }
    
    IEnumerator ResetThrowing()
    {
        yield return new WaitForSeconds(1);
        throwing = false;
        //animator.SetBool(throwHash, false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (action.triggered && numFlares > 0)
        //{
        //    equipmentSwapper.Raise(flareEqipment);
        //    animator.SetBool(throwHash, true);
        //}
        
    }

    public void ThrowFlare()
    {
        numFlares--;
        throwing = false;
        hasThrownFlare = true;
        animator.SetBool(throwHash, false);
        Quaternion q = Quaternion.Euler(-throwAngle, 0f, 0f);
        Vector3 dir = q * lookDir.forward;
        Vector3 pos = lookDir.position;
        if (Physics.Raycast(pos, dir, out RaycastHit hit, collideDist))
        {
            pos = hit.point;
        }
        else
        {
            pos += dir * collideDist;
        }

        Flare go = Instantiate(flarePrefab, pos, Quaternion.identity);

        Rigidbody rb = go.GetComponent<Rigidbody>();
        //rb.velocity = dir * throwForce;
        rb.AddForce(dir * throwForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(torqueForce, 0, torqueForce), ForceMode.Impulse);

        equipmentSwapper.Raise(prev);
    }

    public void SetFlares(int value)
    {
        numFlares = Mathf.Clamp(value, 0, maxNumFlares);
        if (!hasThrownFlare && numFlares > 0)
        {
            TooltipUtil.Display("Press " + key + " to trow flare!", 5);
            hasThrownFlare = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Quaternion q = Quaternion.Euler(-throwAngle, 0f, 0f);
        Vector3 dir = q * lookDir.forward;
        Vector3 pos = lookDir.position;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(pos, dir);
    }
}
