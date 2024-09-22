using UnityEngine;
using UnityEngine.Events;

public class CameraClick : MonoBehaviour
{

    [SerializeField]Camera cam;

    public UnityEvent<Vector3> ClickEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!cam)
            cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Vector3 point = cam.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                ClickEvent.Invoke(hit.point);
            }
        }
    }
}
