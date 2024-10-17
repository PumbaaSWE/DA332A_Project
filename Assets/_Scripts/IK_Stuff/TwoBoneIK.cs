using UnityEngine;

public class TwoBoneIK : MonoBehaviour
{

    [Header("Bones")]
    [Tooltip("Assign tip (i.e. hand), then right click on component >Setup from tip")]
    [SerializeField] private Transform tip;
    [SerializeField] private Transform mid;
    [SerializeField] private Transform root;
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform pole;

    [SerializeField] private float rootElbowRotation;//Rotation offsets
    [SerializeField] private float midElbowRotation;

    float rootLength, midLength;


    void Awake()
    {
        rootLength = (root.position - mid.position).magnitude;
        midLength = (mid.position - tip.position).magnitude;
    }




    private void Solve()
    {

        float a = rootLength;
        float b = midLength;
        float c = Vector3.Distance(root.position, target.position);
        Vector3 en = Vector3.Cross(target.position - root.position, pole.position - root.position);

        Debug.DrawLine(root.position, target.position);
        Debug.DrawLine((root.position + target.position) / 2, mid.position);

        //Set the rotation of the root arm
        root.rotation = Quaternion.LookRotation(target.position - root.position, Quaternion.AngleAxis(rootElbowRotation, mid.position - root.position) * en);
        root.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, mid.localPosition));
        root.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -en) * root.rotation;

        //set the rotation of the mid arm
        mid.rotation = Quaternion.LookRotation(target.position - mid.position, Quaternion.AngleAxis(midElbowRotation, tip.position - mid.position) * en);
        mid.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, tip.localPosition));

        //mid.LookAt(mid, Pole.position - root.position);
        //mid.rotation = Quaternion.AngleAxis(CosAngle(a, b, c), en);
        tip.rotation = target.rotation;

        //function that finds angles using the cosine rule 
        static float CosAngle(float a, float b, float c)
        {
            if (!float.IsNaN(Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (-2 * a * b)) * Mathf.Rad2Deg))
            {
                return Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (2 * a * b)) * Mathf.Rad2Deg;
            }
            else
            {
                return 1;
            }
        }
    }

    void Update()
    {
        Solve();
    }

    [ContextMenu("Setup from tip")]
    public void SetUpFromTip()
    {
        mid = tip.parent;
        root = mid.parent;
        if (target == null)
        {
            target = transform.AddChild(gameObject.name + "_target");
        }
        if (pole == null)
        {
            pole = transform.AddChild(gameObject.name + "_pole");
        }
    }
}
