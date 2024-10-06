using System.Collections;
using System.Threading;
using UnityEngine;

public class LookingEyes : MonoBehaviour
{
    public PlayerDataSO playerData;
    public Transform eyeHead;
    public SlideObject slide;
    public LayerMask layerMask;
    Player player;
    float timer;
    static readonly Collider[] colliders = new Collider[4];
    [SerializeField][Range(0, 1)] float angle = .99f;
    // Start is called before the first frame update
    void Start()
    {
        playerData.NotifyOnPlayerChanged(GetPlayer);
        eyeHead.gameObject.SetActive(false);
        timer = 1;
        enabled = false;
    }

    private void GetPlayer(Transform obj)
    {
        player = obj.GetComponent<Player>();
        //enabled = player != null;

    }

    public void StartLook()
    {
        timer = 1;
        slide.SlideToEnd();
        eyeHead.gameObject.SetActive(true);
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!player) return;

        Vector3 dir = eyeHead.position - player.HeadPos;

        if(dir.sqrMagnitude < 25)
        {
            //Debug.Log("player to close in the area");
            enabled = false;
            slide.SlideToStart();
            StartCoroutine(SetActiveDelay(1));
        }
        float d = Vector3.Dot(dir.normalized, player.LookDir);
        timer -= Time.deltaTime;
        if (d > angle && timer < 0)
        {
            enabled = false;
            slide.SlideToStart();
            StartCoroutine(SetActiveDelay(1));
        }
        if(Physics.CheckSphere(eyeHead.position, 20, layerMask))
        {
            //Debug.Log("flares in the area");
            enabled = false;
            slide.SlideToStart();
            StartCoroutine(SetActiveDelay(1));
        }

        //if (0 > Physics.OverlapSphereNonAlloc(eyeHead.position, 20, colliders, layerMask))
        //{
            
        //}
    }

    public IEnumerator SetActiveDelay(float t)
    {
        yield return new WaitForSeconds(t);
        eyeHead.gameObject.SetActive(false);
    }
}
