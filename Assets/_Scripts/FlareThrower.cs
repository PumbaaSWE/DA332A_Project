using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlareThrower : MonoBehaviour
{
    [SerializeField] private Transform lookDir;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private float collideDist = 1.5f;
    [SerializeField] private float throwForce = 15;
    [SerializeField] private float torqueForce = 1;

    [SerializeField] private int numFlares;
    [SerializeField] private int maxNumFlares = 5;

    public int NumFlares { get { return numFlares; } set { numFlares = Mathf.Clamp(value, 0, maxNumFlares); } }
    public int MaxNumFlares { get { return maxNumFlares; } set { maxNumFlares = Mathf.Min(value, 0); } }


    private bool hasThrownFlare;

    [SerializeField] private PlayerInput playerInput;
    private InputAction action;
    private string key = "<nope>";

    // Start is called before the first frame update
    void Start()
    {
        action = playerInput.actions.FindAction("F");
        key = "[" + action.bindings.First().ToDisplayString() + "]";
        
    }

    // Update is called once per frame
    void Update()
    {
        if (action.triggered && numFlares > 0)
        {
            numFlares--;
            hasThrownFlare = true;
            Quaternion q = Quaternion.Euler(15f, 0f, 0f);
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

            GameObject go = Instantiate(flarePrefab, pos, Quaternion.identity);

            Rigidbody rb = go.GetComponent<Rigidbody>();
            //rb.velocity = dir * throwForce;
            rb.AddForce(dir * throwForce, ForceMode.Impulse);
            rb.AddTorque(new Vector3(torqueForce, 0, torqueForce), ForceMode.Impulse);
        }

        //only call once when you first pick up or smth...
        if(!hasThrownFlare && numFlares > 0)
        {
            TooltipUtil.Display("Press " + key + " to trow flare!", 5);
            hasThrownFlare = true;
        }
    }
}
