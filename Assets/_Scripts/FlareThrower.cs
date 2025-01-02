using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlareThrower : MonoBehaviour
{
    [SerializeField] private Transform lookDir;
    [SerializeField] private Flare flarePrefab;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private GameObject flareArms;
    [SerializeField] private float throwAngle = 15;
    [SerializeField] private float collideDist = 1.5f;
    [SerializeField] private float throwForce = 15;
    [SerializeField] private float torqueForce = 1;

    [SerializeField] private int numFlares;
    [SerializeField] private int maxNumFlares = 5;

    public int NumFlares { get { return numFlares; } set { SetFlares(value); } }
    public int MaxNumFlares { get { return maxNumFlares; } set { maxNumFlares = Mathf.Min(value, 0); } }

    private bool hasThrownFlare;

    [SerializeField] private PlayerInput playerInput;
    private InputAction action;
    private string key = "<nope>";

    AudioSource audioSource;
    [SerializeField]
    private AudioClip clipThrow;

    // Start is called before the first frame update
    void OnEnable()
    {
        action = playerInput.actions.FindAction("F");
        key = "[" + action.bindings.First().ToDisplayString() + "]";
        action.performed += Action_performed;

        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        action.performed -= Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (numFlares > 0)
        {

            weaponHandler.HideWeapons(()=>StartThrow());
            //StartCoroutine(ResetThrowing());
        }
    }

    private void StartThrow()
    {
        flareArms.SetActive(true);
        PlayAudio(clipThrow);
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
        hasThrownFlare = true;
       // animator.SetBool(throwHash, false);
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

      //  equipmentSwapper.Raise(prev);
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

    public void ThrowFinished()
    {
        flareArms.SetActive(false);
        weaponHandler.UnHideWeapons();
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip == null) return;
        if(!audioSource) return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}
