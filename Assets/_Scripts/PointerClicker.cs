using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerClicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.TryGetComponent(out LimbHitbox limbHitbox))
                {
                    limbHitbox.ReciveHit(50);
                }
            }
        }
        
        
    }
}
